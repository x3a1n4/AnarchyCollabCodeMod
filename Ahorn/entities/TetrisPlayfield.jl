module AnarchyCollab2022TetrisPlayfield

using ..Ahorn, Maple

#define properties of entity
@mapdef Entity "AnarchyCollab2022/TetrisPlayfield" TetrisPlayfield(
    x::Integer,
    y::Integer,
    width::Integer=16,
    height::Integer=16,
    ID::Integer=0,
)

#TODO:
const placements = Ahorn.PlacementDict(
    "Tetris Playfield (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        TetrisPlayfield,
        "rectangle"
    )
)

Ahorn.minimumSize(entity::TetrisPlayfield) = 8, 8
Ahorn.resizable(entity::TetrisPlayfield) = true, true

function Ahorn.selection(entity::TetrisPlayfield)
    x, y = Ahorn.position(entity)
    width, height = get(entity.data, "width", 16), get(entity.data, "height", 16)
    #todo
    return Ahorn.Rectangle(x, y, width, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TetrisPlayfield, room::Maple.Room)
    x, y = Ahorn.position(entity)
    width, height = get(entity.data, "width", 16), get(entity.data, "height", 16)
    Ahorn.drawRectangle(ctx, 0, 0, width, height, (1.0, 1.0, 1.0, 0.3))
end

end
