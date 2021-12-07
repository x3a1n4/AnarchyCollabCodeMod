module AnarchyCollab2022PlayerNPC
using ..Ahorn, Maple

@mapdef Entity "AnarchyCollab2022/PlayerDialogNPC" PlayerDialogNPC(x::Integer, y::Integer, flipX::Bool=false, flipY::Bool=false, spriteMode::Int=0, hairColor::String="AC3232", idleAnimation::String="idle", approachStartDistance::Int=0, dialogIds::String="", dialogAnimations::String="", loopDialog::Bool=false)

const placements = Ahorn.PlacementDict(
    "Player Dialog NPC (AnarchyCollab2022)" => Ahorn.EntityPlacement(
        PlayerDialogNPC
    )
)

Ahorn.editingOptions(entity::PlayerDialogNPC) = Dict{String, Any}(
    "spriteMode" => Dict{String, Int}(
        "Madeline" => 0,
        "MadelineNoBackpack" => 1,
        "Badeline" => 2,
        "MadelineAsBadeline" => 3,
        "Playback" => 4
    )
)

function Ahorn.selection(entity::PlayerDialogNPC)
    x, y = Ahorn.position(entity)
    return Ahorn.getSpriteRectangle("characters/player/sitDown00", x, y, sx=get(entity.data, "flipX", false) ? -1 : 1, sy=get(entity.data, "flipY", false) ? -1 : 1, jx=0.5, jy=1.0);
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::PlayerDialogNPC, room::Maple.Room)
    Ahorn.drawSprite(ctx, "characters/player/sitDown00", 0, 0, sx=get(entity.data, "flipX", false) ? -1 : 1, sy=get(entity.data, "flipY", false) ? -1 : 1, jx=0.5, jy=1.0);
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::PlayerDialogNPC, room::Maple.Room)
    dst = get(entity.data, "approachStartDistance", 0)
    if dst > 0
        x, y = Ahorn.position(entity)
        Ahorn.drawRectangle(ctx, x - dst, y - 64, dst * 2, 128, (1.0, 0.0, 0.0, 0.2));
    end
end

end