"use client"

import { useState } from "react"
import { CollectionItem } from "../components/characters-overview"

export function CharacterOverviewExample() {
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
    <div className="rounded-lg border border-dashed bg-muted p-10 dark:bg-background">
      <div className="flex flex-wrap gap-8">
        {characters.map((character) => (
          <CollectionItem
            key={character.id}
            id={character.id}
            name={character.name}
            build={character.build}
            portrait={character.portrait}
            isSelected={selectedCharacter === character.id}
            isHearted={heartedCharacters.includes(character.id)}
            onSelectedChange={(id, isSelected) =>
              setSelectedCharacter(isSelected ? id : null)
            }
            onHeartToggle={(id) => {
              toggleHeart(id)
            }}
            onManage={(id) => {
              console.log(`Manage ${id}`)
            }}
            onDelete={(id) => {
              console.log(`Delete ${id}`)
            }}
          />
        ))}
      </div>
    </div>
  )
}
