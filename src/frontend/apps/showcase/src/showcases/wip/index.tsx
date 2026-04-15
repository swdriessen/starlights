"use client"

import {
  CharacterPortraitCampaignBrief,
  CharacterPortraitCampaignQuestLog,
  CharacterPortraitCampaignRoster,
  CharacterPortraitStory,
  CharacterPortraitVariantDuelist,
  CharacterPortraitVariantImageOnlyCleric,
  CharacterPortraitVariantImageOnlyDragon,
  CharacterPortraitVariantImageOnlyRanger,
  CharacterPortraitVariantImageOnlyRanger2,
  CharacterPortraitVariantImageOnlyRogue,
  CharacterPortraitVariantMage,
  CharacterPortraitVariantScout,
  CharacterPortraitVariantWarden,
} from "./cards/character-portrait"

export default function WipExample() {
  return (
    <div className="3xl:[--gap:--spacing(12)] border-rounded m-10 overflow-x-auto overflow-y-hidden bg-muted contain-[paint] [--gap:--spacing(4)] md:[--gap:--spacing(10)] dark:bg-background">
      <div className="flex w-full min-w-max justify-center">
        <div
          className="grid w-500 grid-cols-5 items-start gap-(--gap) bg-muted p-(--gap) dark:bg-background *:[div]:gap-(--gap)"
          data-slot="capture-target"
        >
          <div className="flex flex-col p-px [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <CharacterPortraitStory />
            <CharacterPortraitCampaignBrief />
          </div>

          <div className="flex flex-col p-px [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <CharacterPortraitCampaignRoster />
            <CharacterPortraitCampaignQuestLog />
          </div>
        </div>
      </div>
      <div className="flex w-full min-w-max justify-center">
        <div
          className="grid w-500 grid-cols-5 items-start gap-(--gap) bg-muted p-(--gap) dark:bg-background *:[div]:gap-(--gap)"
          data-slot="capture-target"
        >
          <div className="flex flex-col p-px [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <CharacterPortraitVariantDuelist />
            <CharacterPortraitVariantMage />
          </div>

          <div className="flex flex-col p-px [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <CharacterPortraitVariantWarden />
            <CharacterPortraitVariantScout />
          </div>

          <div className="flex flex-col p-px [contain-intrinsic-size:380px_1200px] [content-visibility:auto]">
            <CharacterPortraitVariantImageOnlyRogue />
            <CharacterPortraitVariantImageOnlyDragon />
            <CharacterPortraitVariantImageOnlyRanger />
            <CharacterPortraitVariantImageOnlyRanger2 />
            <CharacterPortraitVariantImageOnlyCleric />
          </div>
        </div>
      </div>
    </div>
  )
}
