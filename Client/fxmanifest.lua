fx_version 'cerulean'
games { 'gta5' }

client_scripts {
	'Client.net.dll', 
	'client/target.lua',
	'client/events.lua'
}

server_scripts {
	'Server.net.dll',
	'server/events.lua'
}

dependency 'qb-core'