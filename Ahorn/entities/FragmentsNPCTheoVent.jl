module AnarchyCollab2022PlayerNPC
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/FragmentsNPCTheoVent" FragmentsNPCTheoVent(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Fragments Theo Vent NPC (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        FragmentsNPCTheoVent
    )
)

Ahorn.editingOptions(entity::FragmentsNPCTheoVent) = Dict{String, Any}(
)

function Ahorn.selection(entity::FragmentsNPCTheoVent)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle("characters/theo/theo00", x, y, jx=0.5, jy=1.0)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FragmentsNPCTheoVent, room::Maple.Room)
    Ahorn.drawSprite(ctx, "characters/theo/theo00", 0, 0, jx=0.5, jy=1.0)
end

end