"use client"

import { type ReactNode, useState } from "react"
import { CollectionContainer, CollectionItem } from "./characters-overview"

type ExampleContainerProps = {
  children: ReactNode
  title: string
}

export function ExampleContainer({ children, title }: ExampleContainerProps) {
  return (
    <>
      <h2 className="mx-10 mt-10 mb-2 text-sm font-semibold uppercase">
        {title}
      </h2>
      <div className="relative mx-10 rounded-lg border border-dashed bg-muted p-10 dark:bg-background">
        {children}
      </div>
    </>
  )
}

export function CharacterOverviewExample() {
  const [selectedCharacter, setSelectedCharacter] = useState<string | null>(
    null
  )
  const [heartedCharacters, setHeartedCharacters] = useState<string[]>([
    "catti-brie",
    "bruenor",
  ])

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
    <ExampleContainer title="Character Collection">
      <CollectionContainer>
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
      </CollectionContainer>
    </ExampleContainer>
  )
}
