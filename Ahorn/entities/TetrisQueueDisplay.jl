module AnarchyCollab2022TetrisQueueDisplay

using ..Ahorn, Maple

#define properties of entity
@mapdef Entity "AnarchyCollab2022/TetrisQueueDisplay" TetrisQueueDisplay(
    x::Integer,
    y::Integer,
    ID::Integer=0,
)

#TODO:
const placements = Ahorn.PlacementDict(
    "Tetris Queue Display (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        TetrisQueueDisplay,
    )
)


function Ahorn.selection(entity::TetrisQueueDisplay)
    x, y = Ahorn.position(entity)
    #todo
    return Ahorn.Rectangle(x, y, 40, 128)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TetrisQueueDisplay, room::Maple.Room)
    Ahorn.drawSprite(ctx, "ahorn/Tetris/queue_display", 20, 64)
end

end
