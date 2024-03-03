CreateThread(function()
    -- Starter Ped
    local pedModel = `s_m_y_dockwork_01`
    RequestModel(pedModel)
    while not HasModelLoaded(pedModel) do Wait(10) end
    local ped = CreatePed(0, pedModel, 1197.01, -3252.36, 6.1, 87.16, false, false)
    TaskStartScenarioInPlace(ped, 'WORLD_HUMAN_CLIPBOARD', true) -- play scenario (pastebin.com/6mrYTdQv)
    FreezeEntityPosition(ped, true)
    SetEntityInvincible(ped, true)
    SetBlockingOfNonTemporaryEvents(ped, true) -- make obvlivious to everything going on
    -- Target
    exports['qb-target']:AddTargetEntity(ped, {
        options = {
            {
                type = 'client',
                event = 'svgl-freight:client:TargetSelected',
                icon = '',
                label = 'Trucking Manager',
            }
        },
        distance = 2.0
    })
end)