"use client"

import { useState } from "react"
import { CharacterOverviewItem } from "../components/characters-overview"
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

export function Example() {
  const [selectedCharacter, setSelectedCharacter] = useState<string | null>(
    "catti-brie"
  )
  const [heartedCharacters, setHeartedCharacters] = useState<string[]>([])

  const characters = [
    {
      id: "drizzt",
      name: "Drizzt Do'Urden",
      build: "Level 11 Gloom Stalker Ranger",
      portrait: "/portraits/portrait-10.png",
    },
    {
      id: "catti-brie",
      name: "Catti-brie",
      build: "Level 10 Arcane Archer Fighter",
      portrait: "/portraits/portrait-4.jpg",
    },
    {
      id: "bruenor",
      name: "Bruenor Battlehammer",
      build: "Level 10 Champion Fighter",
      portrait: "/portraits/portrait-15.png",
    },
    {
      id: "jarlaxle",
      name: "Jarlaxle Baenre",
      build: "Level 11 Swashbuckler Rogue",
      portrait: "/portraits/portrait-1.jpg",
    },
  ]

  const toggleHeart = (id: string) => {
    setHeartedCharacters((current) =>
      current.includes(id)
        ? current.filter((value) => value !== id)
        : [...current, id]
    )
  }

  return (
    <div className="m-10 rounded-lg border border-dashed bg-muted p-10 dark:bg-background">
      <div className="flex flex-wrap gap-8">
        {characters.map((character) => (
          <CharacterOverviewItem
            key={character.id}
            name={character.name}
            build={character.build}
            portrait={character.portrait}
            isSelected={selectedCharacter === character.id}
            isHearted={heartedCharacters.includes(character.id)}
            onSelectedChange={(isSelected) =>
              setSelectedCharacter(isSelected ? character.id : null)
            }
            onHeart={() => {
              toggleHeart(character.id)
            }}
            onView={() => {
              console.log(`View ${character.name}`)
            }}
            onDelete={() => {
              console.log(`Delete ${character.name}`)
            }}
          />
        ))}
      </div>
    </div>
  )
}

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
