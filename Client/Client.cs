using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.NaturalMotion;

namespace Client
{
    public class Client : BaseScript
    {
        private bool jobStarted;
        private bool showDropoffMarker;
        private bool trailerPickedUp;
        private bool trailerMessageSent;
        private bool trailerDelivered;
        private bool truckReturned;

        private Vector3 dropoffDestination;
        private Vector3 dropoffZone;

        private Blip dropoffBlip;
        private Blip truckReturnBlip;
        private Blip trailerBlip;

        private Vehicle assignedTrailer;
        private Vehicle assignedTruck;

        private Route selectedRoute;

        private int jobPayment;

        private static readonly Random rnd = new Random();

        public Client()
        {
            Tick += OnTick;
            Blip mapBlip = World.CreateBlip(new Vector3(1196.6f, -3252.03f, 7.1f));
            API.SetBlipSprite(mapBlip.Handle, 477);
            mapBlip.Color = BlipColor.Blue;
            mapBlip.Name = "Freight Delivery";
            mapBlip.Scale = 0.5f;
            API.SetBlipAsShortRange(mapBlip.Handle, true);
        }

        private async Task OnTick()
        {
            if (jobStarted) 
            {
                if (showDropoffMarker)
                {
                    API.DrawMarker(1, dropoffZone.X, dropoffZone.Y, dropoffZone.Z - 1, 0, 0, 0, 0, 0, 0, 5, 5, 5, 255, 255, 0, 100, false, true, 2, false, null, null, false);
                    CheckForTrailerInDropOffZone();
                }

                if (!trailerPickedUp)
                {
                    AttachTruckToTrailerWhenClose();
                }

                if (!trailerMessageSent)
                {
                    WaitForPedToEnterTruck();
                }

                if (trailerDelivered && API.IsVehicleAttachedToTrailer(assignedTruck.Handle))
                {
                    API.DetachVehicleFromTrailer(assignedTruck.Handle);
                }
            }
        }

        [EventHandler("svgl-freight:client:StartJob")]
        private void StartTruckingJob()
        {
            // initialize
            jobStarted = true;
            selectedRoute = Routes.GetRandomRoute();
            dropoffDestination = selectedRoute.Destination;
            dropoffZone = selectedRoute.DropoffLocation;
            float distance = Vector3.Distance(Game.PlayerPed.Position, dropoffDestination);
            jobPayment = (int)Math.Ceiling(distance * 0.5); // 0.5 rate/meter

            TriggerServerEvent("svgl-freight:server:StartJob");
            TriggerServerEvent("svgl:server:RemoveMoney", "bank", 500, "Truck deposit");
            TriggerEvent("svgl-freight:client:SpawnTruck");
        }

        [EventHandler("svgl-freight:client:CompleteJob")]
        private void CompleteTruckingJob()
        {
            if (trailerDelivered)
            {
                TriggerServerEvent("svgl:server:AddMoney", "bank", jobPayment, "Truck Delivery Success");
                TriggerServerEvent("svgl:Notification", $"Delivery complete, your delivery earned you ${jobPayment}", "success", 5000);
            }
            
            if (truckReturned)
            {
                TriggerServerEvent("svgl:server:AddMoney", "bank", 500, "Truck Deposit Refund");
                TriggerServerEvent("svgl:Notification", "Truck return confirmed, we've refunded your deposit", "success", 5000);
            }

            if (!truckReturned && !trailerDelivered)
            {
                TriggerServerEvent("svgl:Notification", "Why do we even hire you?", "error", 5000);
            }

            TriggerServerEvent("svgl-freight:server:CompleteJob");

            ResetJob();
        }

        [EventHandler("svgl-freight:client:SpawnTruck")]
        private void OnTruckSpawn()
        {
            TriggerServerEvent("svgl:Notification", "A driver is bringing your truck", "primary", 5000);
            LoadTruckAndTrailer();
        }

        [EventHandler("svgl-freight:client:TargetSelected")]
        private void OnTargetSelected()
        {
            if (jobStarted)
            {
                truckReturned = IsTruckNearby();
            }
            TriggerEvent("svgl-freight:client:OpenMenu", jobStarted);
        }

        [EventHandler("svgl-freight:client:DeliveryMade")]
        private void OnDeliveryMade()
        {
            trailerDelivered = true;
            TriggerServerEvent("svgl:Notification", "Return to the depot to get paid", "success", 5000);
            //TriggerServerEvent("svgl:server:AddMoney", "bank", 500, "Truck Delivery Success");

            showDropoffMarker = false;
            dropoffBlip?.Delete();
            Vector3 truckReturnDestination = new Vector3(1174.18f, -3251.45f, 4.86f);

            truckReturnBlip = World.CreateBlip(truckReturnDestination);
            truckReturnBlip.Sprite = BlipSprite.Standard;
            truckReturnBlip.Color = BlipColor.Yellow;
            truckReturnBlip.ShowRoute = true;
        }

        private async void LoadTruckAndTrailer()
        {
            string trailerModelString = selectedRoute.GetRandomTrailer();
            Debug.WriteLine(trailerModelString);
            uint truckModel = (uint)API.GetHashKey("hauler");
            uint trailerModel = (uint)API.GetHashKey(trailerModelString);

            // load trucks
            bool truckModelLoaded = await LoadModel(truckModel);
            bool trailerModelLoaded = await LoadModel(trailerModel);

            // error handling
            if (!truckModelLoaded && !trailerModelLoaded)
            {
                TriggerServerEvent("svgl:Notification", "No trucks available", "error", 5000);
                return;
            }

            assignedTruck = new Vehicle(API.CreateVehicle(truckModel, 1189.57f, -3336.12f, 4.82f, 88.86f, true, false));
            //assignedTrailer = new Vehicle(API.CreateVehicle(trailerModel, 1195.68f, -3259.86f, 4.82f, 88.86f, true, false));

            Vector4 trailerSpawnPosition = GetTrailerSpawnPosition();
            Debug.WriteLine(trailerSpawnPosition.ToString());
            assignedTrailer = new Vehicle(API.CreateVehicle(trailerModel, trailerSpawnPosition.X, trailerSpawnPosition.Y, trailerSpawnPosition.Z, trailerSpawnPosition.W, true, false));

            if (assignedTruck != null && assignedTruck.Exists())
            {
                API.SetVehicleFixed(assignedTruck.Handle);
            }

            if (assignedTrailer != null && assignedTrailer.Exists())
            {
                trailerBlip = World.CreateBlip(assignedTrailer.Position);
                API.SetBlipSprite(trailerBlip.Handle, 479);
                trailerBlip.Color = BlipColor.Blue;
                trailerBlip.Scale = 0.25f;
            }
            
            // load ped
            uint pedModel = (uint)API.GetHashKey("s_m_y_dockwork_01");
            bool pedModelLoaded = await LoadModel(pedModel);

            // error handling 
            if (!pedModelLoaded)
            {
                TriggerServerEvent("svgl:Notification", "No drivers available", "error", 5000);
                return;
            }

            // old trailer came attached to truck
            //API.AttachVehicleToTrailer(assignedTruck.Handle, assignedTrailer.Handle, 1.0f);

            string plateNumber = API.GetVehicleNumberPlateText(assignedTruck.Handle);
            TriggerEvent("qb-vehiclekeys:client:AddKeys", plateNumber);

            int pedHandle = API.CreatePedInsideVehicle(assignedTruck.Handle, 26, pedModel, -1, true, false);
            Ped driver = new Ped(pedHandle);

            Vector3 dropOffZone = new Vector3(1172.68f, -3259.86f, 4.82f);
            Vector3 walkPosition = new Vector3(1197.15f, -3253.41f, 7.1f);
            
            var sequence = new TaskSequence();
            sequence.AddTask.DriveTo(assignedTruck, dropOffZone, 5.0f, 10.0f);
            sequence.AddTask.LeaveVehicle(assignedTruck, LeaveVehicleFlags.LeaveDoorOpen);
            sequence.AddTask.GoTo(walkPosition);
            sequence.AddTask.Wait(500);
            sequence.Close();

            driver.Task.PerformSequence(sequence);

            DeleteNPCAfterDelay(pedHandle, 37000);
        }

        private async Task<bool> LoadModel(uint model)
        {
            API.RequestModel(model);
            int attempts = 0;

            while (!API.HasModelLoaded(model) && attempts < 10)
            {
                await BaseScript.Delay(100);
                attempts++;
            }

            return API.HasModelLoaded(model);
        }

        private async void DeleteNPCAfterDelay(int pedHandle, int delay)
        {
            await BaseScript.Delay(delay);
            if (API.DoesEntityExist(pedHandle))
            {
                API.DeletePed(ref pedHandle);
            }
        }

        private void CheckForTrailerInDropOffZone()
        {
            if (assignedTrailer == null || !assignedTrailer.Exists()) { return; }

            Vector3 trailerPosition = assignedTrailer.Position;
            float distanceToDropoffZone = trailerPosition.DistanceToSquared(dropoffZone);
            float dropoffZoneRadius = 5.0f;

            if (distanceToDropoffZone <= dropoffZoneRadius * dropoffZoneRadius)
            {
                API.DetachVehicleFromTrailer(assignedTruck.Handle);
                TriggerEvent("svgl-freight:client:DeliveryMade");
            }
        }

        private void WaitForPedToEnterTruck()
        {
            if (assignedTruck == null || !assignedTruck.Exists()) {  return; }
            var player = Game.PlayerPed;
            if (player.IsInVehicle(assignedTruck))
            {
                trailerMessageSent = true;
                TriggerServerEvent("svgl:Notification", "Attach the designated trailer to get delivery location", "primary", 5000);
            }
        }

        private void AttachTruckToTrailerWhenClose()
        {
            if (assignedTruck == null || !assignedTruck.Exists() || assignedTrailer == null || !assignedTrailer.Exists()) { return; }

            if (assignedTruck != null && assignedTruck.Exists())
            {
                if (API.IsVehicleAttachedToTrailer(assignedTruck.Handle))
                {
                    TriggerServerEvent("svgl:Notification", "Deliver the trailer to the customer", "primary", 5000);
                    trailerPickedUp = true;
                    SetDropoffDestination();
                    trailerBlip?.Delete();
                }
            }
        }

        private void SetDropoffDestination()
        {
            dropoffBlip = World.CreateBlip(dropoffDestination);
            dropoffBlip.Sprite = BlipSprite.Standard;
            dropoffBlip.Color = BlipColor.Yellow;
            dropoffBlip.ShowRoute = true;

            showDropoffMarker = true;
        }

        private bool IsTruckNearby()
        {
            if (assignedTruck != null && assignedTruck.Exists())
            {
                Vector3 playerPos = Game.PlayerPed.Position;
                float truckProximityThreshold = 50.0f;
                float distance = playerPos.DistanceToSquared(assignedTruck.Position);
                return distance <= truckProximityThreshold * truckProximityThreshold;
            }

            return false;
        }

        private void ResetJob()
        {
            jobStarted = false;
            showDropoffMarker = false;
            trailerPickedUp = false;
            trailerMessageSent = false;
            trailerDelivered = false;
            truckReturned = false;

            dropoffBlip?.Delete();
            truckReturnBlip?.Delete();
            trailerBlip?.Delete();

            if (assignedTrailer?.Exists() == true) { assignedTrailer?.Delete(); }
            if (assignedTruck?.Exists() == true) { assignedTruck?.Delete(); }
        }

        private Vector4 GetTrailerSpawnPosition()
        {
            List<Vector4> spawnPosition = new List<Vector4>
            {
                new Vector4(1274.19f, -3194.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3202.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3208.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3216.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3222.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3230.6f, 5.1f, 90f),
                new Vector4(1274.19f, -3237.6f, 5.1f, 90f),
            };
            
            int index = rnd.Next(spawnPosition.Count);
            return spawnPosition[index];
        }
    }
}
