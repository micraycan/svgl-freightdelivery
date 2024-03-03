-- C# wrapper function events --
local QBCore = exports['qb-core']:GetCoreObject()

-- remove money event
RegisterNetEvent('svgl:server:RemoveMoney', function(type, amount, reason)
	local Player = QBCore.Functions.GetPlayer(source)
	Player.Functions.RemoveMoney(type, amount, reason)
end)

-- add money event
RegisterNetEvent('svgl:server:AddMoney', function(type, amount, reason)
	local Player = QBCore.Functions.GetPlayer(source)
	Player.Functions.AddMoney(type, amount, reason)
end)