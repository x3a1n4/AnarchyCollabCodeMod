module AnarchyCollab2022ReflexiveThrowable
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/ReflexiveThrowable" ReflexiveThrowable(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Reflexive Throwable (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        ReflexiveThrowable
    )
)

sprite = "characters/player/sitDown00.png"

function Ahorn.selection(entity::ReflexiveThrowable)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y, jx=0.5, jy=1);
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ReflexiveThrowable, room::Maple.Room)
    Ahorn.drawSprite(ctx, sprite, 0, 0, jx=0.5, jy=1);
end

end