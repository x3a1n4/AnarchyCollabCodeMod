module AnarchyCollab2022DreamBounceRefill
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/DreamBounceRefill" DreamBounceRefill(x::Integer, y::Integer, doubleRefill::Bool=false, respawnDelay::Float32=Float32(2.5))

const placements = Ahorn.PlacementDict(
    "Dream Bounce Refill (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        DreamBounceRefill,
        "point",
        Dict{String, Any}("doubleRefill" => false)
    ),
    "Double Dream Bounce Refill (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        DreamBounceRefill,
        "point",
        Dict{String, Any}("doubleRefill" => true)
    )
)

spriteOne = "objects/refill/idle00"
spriteTwo = "objects/refillTwo/idle00"

function Ahorn.selection(entity::DreamBounceRefill)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle(get(entity.data, "doubleRefill", false) ? spriteTwo : spriteOne, x, y);
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DreamBounceRefill, room::Maple.Room)
    Ahorn.drawSprite(ctx, get(entity.data, "doubleRefill", false) ? spriteTwo : spriteOne, 0, 0, tint=(0.55, 0.0, 0.55, 1.0));
end

end