module AnarchyCollab2022TetrisScreen

using ..Ahorn, Maple

#define properties of entity
@mapdef Entity "AnarchyCollab2022/TetrisScreen" TetrisScreen(
    x::Integer, 
    y::Integer, 
    ITiles::String="1",
    JTiles::String="2",
    LTiles::String="3",
    OTiles::String="4",
    STiles::String="5",
    TTiles::String="6",
    ZTiles::String="7"
    )

    #TODO:
const placements = Ahorn.PlacementDict(
    "Tetris Screen (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        TetrisScreen
    )
)

Ahorn.editingOptions(entity::TetrisScreen) = Dict{String, Any}(
    "ITiles" => Ahorn.tiletypeEditingOptions(),
    "JTiles" => Ahorn.tiletypeEditingOptions(),
    "LTiles" => Ahorn.tiletypeEditingOptions(),
    "OTiles" => Ahorn.tiletypeEditingOptions(),
    "STiles" => Ahorn.tiletypeEditingOptions(),
    "TTiles" => Ahorn.tiletypeEditingOptions(),
    "ZTiles" => Ahorn.tiletypeEditingOptions()
)

function Ahorn.selection(entity::TetrisScreen)
    x, y = Ahorn.position(entity)
    

    #todo
    return Ahorn.Rectangle(x-40, y-80, 80, 160)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::TetrisScreen, room::Maple.Room)
    Ahorn.drawSprite(ctx, "ahorn/TetrisScreen/tetris_screen", 0, -4)

    #Ahorn.drawRectangle(ctx, 0, 0, 10, 10, (1.0, 1.0, 1.0, 1.0))
end

end
