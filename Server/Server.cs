using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Server
{
    public class Server : BaseScript
    {
        public Server()
        {
            EventHandlers["svgl-freight:server:StartJob"] += new Action<Player>(OnStartJob);
            EventHandlers["svgl-freight:server:CompleteJob"] += new Action<Player>(OnCompleteJob);
            EventHandlers["svgl:Notification"] += new Action<Player, string, string, int>(OnNotify);
        }

        private void OnStartJob([FromSource] Player player)
        {
            Debug.WriteLine($"{player.Name} starting freight job");
        }

        private void OnCompleteJob([FromSource] Player player)
        {

        }

        private void OnNotify([FromSource] Player player, string message, string textType, int length)
        {
            TriggerClientEvent(player, "QBCore:Notify", message, textType, length);
        }
    }
}
