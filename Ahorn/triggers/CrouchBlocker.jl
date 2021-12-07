module AnarchyCollab2022CrouchBlocker
using ..Ahorn, Maple

@mapdef Trigger "AnarchyCollab2022/CrouchBlocker" CrouchBlocker(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight, blockGlobal::Bool=false)

const placements = Ahorn.PlacementDict(
    "Crouch Blocker (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        CrouchBlocker,
        "rectangle"
    )
)

end