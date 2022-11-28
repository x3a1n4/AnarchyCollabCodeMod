module AnarchyCollab2022TetrisManager

using ..Ahorn, Maple

#define properties of entity
@mapdef Entity "AnarchyCollab2022/TetrisManager" TetrisManager(
    x::Integer,
    y::Integer,
    ID::Integer=0,

    ITiles::String="1",
    JTiles::String="2",
    LTiles::String="3",
    OTiles::String="4",
    STiles::String="5",
    TTiles::String="6",
    ZTiles::String="7",
    
    Time::Number=120.0,
    LineCount::Integer=40,

    EditTime::Number=1.0,
    DropSpeed::Number=1.0
)

#TODO:
const placements = Ahorn.PlacementDict(
    "Tetris Manager (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        TetrisManager
    )
)

Ahorn.editingOptions(entity::TetrisManager) = Dict{String, Any}(
    "ITiles" => Ahorn.tiletypeEditingOptions(),
    "JTiles" => Ahorn.tiletypeEditingOptions(),
    "LTiles" => Ahorn.tiletypeEditingOptions(),
    "OTiles" => Ahorn.tiletypeEditingOptions(),
    "STiles" => Ahorn.tiletypeEditingOptions(),
    "TTiles" => Ahorn.tiletypeEditingOptions(),
    "ZTiles" => Ahorn.tiletypeEditingOptions()
)

sprite = "ahorn/Tetris/tetris_manager"

Ahorn.nodeLimits(entity::TetrisManager) = 0, -1

function Ahorn.selection(entity::TetrisManager)
    x, y = Ahorn.position(entity)

    res = Ahorn.Rectangle[Ahorn.getSpriteRectangle(sprite, x, y)]

    nodes = get(entity.data, "nodes", ())

    for node in nodes
        nx, ny = node
        push!(res, Ahorn.Rectangle(nx - 8, ny - 8, 16, 16))
    end

    #todo
    return res
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::TetrisManager)
    x, y = Ahorn.position(entity)

    for node in get(entity.data, "nodes", ())
        nx, ny = node

        Ahorn.drawLines(ctx, Tuple{Number, Number}[(x, y), (nx, ny)], Ahorn.colors.selection_selected_fc)
    end
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TetrisManager, room::Maple.Room)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end
