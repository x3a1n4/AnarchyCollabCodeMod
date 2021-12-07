module AnarchyCollab2022PokeSpikes
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/PokeSpikes" PokeSpikes(x::Integer, y::Integer, width::Integer=Maple.defaultSpikeWidth, height::Integer=Maple.defaultSpikeHeight, direction::Integer=0, type::String="default", retractedDistance::Float32=Float32(4.0), permanent::Bool=false)

const placements = Ahorn.PlacementDict(
    "Poke Spikes (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        PokeSpikes,
        "rectangle"
    )
)

const offsets = Dict{Int, Tuple{Int, Int}}(
    0 => (0, -1),
    1 => (0, 0),
    2 => (-1, 0),
    3 => (0, 0)
)

const rotations = Dict{Int, Number}(
    0 => 0,
    1 => pi,
    3 => pi / 2,
    2 => pi * 3 / 2
)

Ahorn.editingOptions(entity::PokeSpikes) = Dict{String, Any}(
    "direction" => Dict{String, Int}(zip(Maple.spike_directions, 0:3)),
    "type" => Maple.spike_types
)

function Ahorn.resizable(entity::PokeSpikes)
    dir = get(entity.data, "direction", 0)
    return (dir == 0 || dir == 1) ? (true, false) : (false, true)
end

function Ahorn.minimumSize(entity::PokeSpikes)
    return (8, 8)
end

function Ahorn.selection(entity::PokeSpikes)
    x, y = Ahorn.position(entity)
    dir = get(entity.data, "direction", 0)
    width, height = get(entity.data, "width", Maple.defaultSpikeWidth), get(entity.data, "height", Maple.defaultSpikeHeight)

    return Ahorn.Rectangle(x, y, width, height);
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::PokeSpikes, room::Maple.Room)
    x, y = Ahorn.position(entity)
    dir = get(entity.data, "direction", 0)
    type = get(entity.data, "type", "default")
    width, height = get(entity.data, "width", Maple.defaultSpikeWidth), get(entity.data, "height", Maple.defaultSpikeHeight)

    offX, offY = offsets[dir]
    for ox in 0:8:width - 8, oy in 0:8:height - 8
        Ahorn.drawSprite(ctx, "danger/spikes/$(type)_$(lowercase(Maple.spike_directions[dir+1]))00", ox + offX, oy + offY, jx=0, jy=0)
    end
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::PokeSpikes)
    x, y = Ahorn.position(entity)
    dir = get(entity.data, "direction", 0)

    width, height = get(entity.data, "width", Maple.defaultSpikeWidth), get(entity.data, "height", Maple.defaultSpikeHeight)

    cx, cy = x + floor(Int, width / 2), y + floor(Int, height / 2)
    theta = rotations[dir] - pi / 2
    Ahorn.drawArrow(ctx, cx, cy, cx + cos(theta) * 24, cy + sin(theta) * 24, Ahorn.colors.selection_selected_fc, headLength=6)
end

end