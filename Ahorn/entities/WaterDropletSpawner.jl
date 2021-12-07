module AnarchyCollab2022WaterDropletSpawner
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/WaterDropletSpawner" WaterDropletSpawner(x::Integer, y::Integer, width::Integer=8, height::Integer=1, interval::Float32=1f0)

const placements = Ahorn.PlacementDict(
    "Water Droplet Spawner (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        WaterDropletSpawner,
        "rectangle"
    )
)

Ahorn.minimumSize(entity::WaterDropletSpawner) = 1, 0
Ahorn.resizable(entity::WaterDropletSpawner) = true, false

Ahorn.selection(entity::WaterDropletSpawner) = Ahorn.getEntityRectangle(entity)

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::WaterDropletSpawner, room::Maple.Room)
    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 1))

    Ahorn.drawRectangle(ctx, 0, 0, width, height, (0.0, 0.0, 1.0, 0.3), (0.0, 0.0, 1.0, 0.7))
end

end