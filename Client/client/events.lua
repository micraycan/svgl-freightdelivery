RegisterNetEvent('svgl-freight:client:OpenMenu', function(data)
	local onJob = not data

	exports['qb-menu']:openMenu({
		{
			header = "Trucking Job inc. or something",
			icon = 'fas fa-code',
			isMenuHeader = true,
		},
		{
			header = 'Start Delivery',
			txt = 'Requires $500 deposit, deliver the trailer to the customer',
			icon = 'fas fa-code-merge',
			hidden = not onJob,
			params = {
				event = 'svgl-freight:client:StartJob'
			}
		},
		{
			header = 'Finish Job',
			txt = 'Truck deposit will be refunded if assigned truck is nearby',
			icon = 'fas fa-code-pull-request',
			hidden = onJob,
			params = {
				event = 'svgl-freight:client:CompleteJob'
			}
		}
	})
end)

