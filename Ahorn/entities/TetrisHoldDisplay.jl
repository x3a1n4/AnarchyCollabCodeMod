module AnarchyCollab2022TetrisHoldDisplay

using ..Ahorn, Maple

#define properties of entity
@mapdef Entity "AnarchyCollab2022/TetrisHoldDisplay" TetrisHoldDisplay(
    x::Integer,
    y::Integer,
    ID::Integer=0,
)

#TODO:
const placements = Ahorn.PlacementDict(
    "Tetris Hold Display (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        TetrisHoldDisplay,
    )
)


function Ahorn.selection(entity::TetrisHoldDisplay)
    x, y = Ahorn.position(entity)
    #todo
    return Ahorn.Rectangle(x, y, 40, 32)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TetrisHoldDisplay, room::Maple.Room)
    Ahorn.drawSprite(ctx, "ahorn/Tetris/hold_display", 20, 16)
end

end
