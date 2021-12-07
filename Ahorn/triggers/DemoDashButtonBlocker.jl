module AnarchyCollab2022DemoDashButtonBlocker
using ..Ahorn, Maple

@mapdef Trigger "AnarchyCollab2022/DemoDashButtonBlocker" DemoDashButtonBlocker(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, blockGlobal::Bool=false)

const placements = Ahorn.PlacementDict(
    "DemoDash Button Blocker (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        DemoDashButtonBlocker,
        "rectangle"
    )
)

end