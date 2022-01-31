module AnarchyCollab2022FragmentsStarJumpBlock

using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/FragmentsStarJumpBlock" FragmentsStarJumpBlock(
  x::Integer, y::Integer,
  width::Integer=16, height::Integer=16,
  sinks::Bool=true
)

@mapdef Entity "AnarchyCollab2022/FragmentsStarJumpController" FragmentsStarJumpController(
  x::Integer, y::Integer
)

const placements = Ahorn.PlacementDict(
    "Fragments Star Jump Block (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        FragmentsStarJumpBlock,
        "rectangle"
    ),
    "Fragments Star Climb Graphics Controller (AnarchyCollab2022)" => Ahorn.EntityPlacement(
      FragmentsStarJumpController,
    ),
)

Ahorn.minimumSize(entity::FragmentsStarJumpBlock) = 8, 8
Ahorn.resizable(entity::FragmentsStarJumpBlock) = true, true

Ahorn.selection(entity::FragmentsStarJumpBlock) = Ahorn.getEntityRectangle(entity)

function Ahorn.selection(entity::FragmentsStarJumpController)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x - 12, y - 12, 24, 24)
end

function getStarjumpRectangles(room::Maple.Room)
    entities = filter(e -> e.name == "AnarchyCollab2022/FragmentsStarJumpBlock", room.entities)
    rects = Ahorn.Rectangle[
        Ahorn.Rectangle(
            Int(get(e.data, "x", 0)),
            Int(get(e.data, "y", 0)),
            Int(get(e.data, "width", 8)),
            Int(get(e.data, "height", 8))
        ) for e in entities
    ]

    return rects
end

# Is there a star block we should connect to at the offset?
function noAdjacent(entity::FragmentsStarJumpBlock, ox::Integer, oy::Integer, rects::Array{Ahorn.Rectangle, 1})
    x, y = Ahorn.position(entity)

    rect = Ahorn.Rectangle(x + ox + 4, y + oy + 4, 1, 1)

    return !any(Ahorn.checkCollision.(rects, Ref(rect)))
end

const fillColor = (255, 255, 255, 255) ./ 255
const corners = String[
    "objects/starjumpBlock/corner00.png",
    "objects/starjumpBlock/corner01.png",
    "objects/starjumpBlock/corner02.png",
    "objects/starjumpBlock/corner03.png"
]
const edgeHs = String[
    "objects/starjumpBlock/edgeH00.png",
    "objects/starjumpBlock/edgeH01.png",
    "objects/starjumpBlock/edgeH02.png",
    "objects/starjumpBlock/edgeH03.png",
]
const edgeVs = String[
    "objects/starjumpBlock/edgeV00.png",
    "objects/starjumpBlock/edgeV01.png",
    "objects/starjumpBlock/edgeV02.png",
    "objects/starjumpBlock/edgeV03.png",
]

function renderStarjumpBlock(ctx::Ahorn.Cairo.CairoContext, entity::FragmentsStarJumpBlock, room::Maple.Room)
    starJumpRectangles = getStarjumpRectangles(room)
    rng = Ahorn.getSimpleEntityRng(entity)

    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))

    # Horizontal Border
    w = 8
    while w < width - 8
        if noAdjacent(entity, w, -8, starJumpRectangles)
            edge = rand(rng, edgeHs)
            Ahorn.drawSprite(ctx, edge, w + 4, 4)
        end

        if noAdjacent(entity, w, height, starJumpRectangles)
            texture = rand(rng, edgeHs)
            Ahorn.drawSprite(ctx, texture, w + 4, height - 4, sy=-1)
        end

        w += 8
    end

    # Vertical Border
    h = 8
    while h < height - 8
        if noAdjacent(entity, -8, h, starJumpRectangles)
            texture = rand(rng, edgeVs)
            Ahorn.drawSprite(ctx, texture, 4, h + 4, sx=-1)
        end

        if noAdjacent(entity, width, h, starJumpRectangles)
            texture = rand(rng, edgeVs)
            Ahorn.drawSprite(ctx, texture, width - 4, h + 4)
        end

        h += 8
    end

    # Top Left Corner
    if noAdjacent(entity, -8, 0, starJumpRectangles) && noAdjacent(entity, 0, -8, starJumpRectangles)
        corner = rand(rng, corners)
        Ahorn.drawSprite(ctx, corner, 4, 4, sx=-1)

    elseif noAdjacent(entity, -8, 0, starJumpRectangles)
        corner = rand(rng, edgeVs)
        Ahorn.drawSprite(ctx, corner, 4, 4, sx=-1)

    elseif noAdjacent(entity, 0, -8, starJumpRectangles)
        corner = rand(rng, edgeHs)
        Ahorn.drawSprite(ctx, corner, 4, 4, sx=-1)
    end

    # Top Right Corner
    if noAdjacent(entity, width, 0, starJumpRectangles) && noAdjacent(entity, width - 8, -8, starJumpRectangles)
        corner = rand(rng, corners)
        Ahorn.drawSprite(ctx, corner, width - 4, 4)

    elseif noAdjacent(entity, width, 0, starJumpRectangles)
        corner = rand(rng, edgeVs)
        Ahorn.drawSprite(ctx, corner, width - 4, 4)

    elseif noAdjacent(entity, width - 8, -8, starJumpRectangles)
        corner = rand(rng, edgeHs)
        Ahorn.drawSprite(ctx, corner, width - 4, 4)
    end

    # Bottom Left Corner
    if noAdjacent(entity, -8, height - 8, starJumpRectangles) && noAdjacent(entity, 0, height, starJumpRectangles)
        corner = rand(rng, corners)
        Ahorn.drawSprite(ctx, corner, 4, height - 4, sx=-1, sy=-1)

    elseif noAdjacent(entity, -8, height - 8, starJumpRectangles)
        corner = rand(rng, edgeVs)
        Ahorn.drawSprite(ctx, corner, 4, height - 4, sx=-1, sy=-1)

    elseif noAdjacent(entity, 0, height, starJumpRectangles)
        corner = rand(rng, edgeHs)
        Ahorn.drawSprite(ctx, corner, 4, height - 4, sx=-1, sy=-1)
    end

    # Bottom Right Corner
    if noAdjacent(entity, width, height - 8, starJumpRectangles) && noAdjacent(entity, width - 8, height, starJumpRectangles)
        corner = rand(rng, corners)
        Ahorn.drawSprite(ctx, corner, width - 4, height - 4, sy=-1)

    elseif noAdjacent(entity, width, height - 8, starJumpRectangles)
        corner = rand(rng, edgeVs)
        Ahorn.drawSprite(ctx, corner, width - 4, height - 4, sy=-1)

    elseif noAdjacent(entity, width - 8, height, starJumpRectangles)
        corner = rand(rng, edgeHs)
        Ahorn.drawSprite(ctx, corner, width - 4, height - 4, sy=-1)
    end
end

Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FragmentsStarJumpBlock, room::Maple.Room) = renderStarjumpBlock(ctx, entity, room)
Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::FragmentsStarJumpController, room::Maple.Room) = Ahorn.drawImage(ctx, Ahorn.Assets.northernLights, -12, -12)

end