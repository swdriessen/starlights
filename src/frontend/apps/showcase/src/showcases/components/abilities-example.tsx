import { useMemo, useState } from "react"
import {
  Button,
  ButtonGroup,
  Card,
  CardContent,
  CardFooter,
  Separator,
} from "ui-framework"
import { ToggleGroup, ToggleGroupItem } from "ui-framework/toggle-group"

import {
  AArrowDownIcon,
  AArrowUpIcon,
  StretchHorizontalIcon,
  StretchVerticalIcon,
} from "lucide-react"
import { AbilitiesGroup } from "./abilities"

const abilityDefinitions = [
  { key: "str", name: "Strength", abbreviation: "STR", base: 15 },
  { key: "dex", name: "Dexterity", abbreviation: "DEX", base: 14 },
  { key: "con", name: "Constitution", abbreviation: "CON", base: 13 },
  { key: "int", name: "Intelligence", abbreviation: "INT", base: 12 },
  { key: "wis", name: "Wisdom", abbreviation: "WIS", base: 10 },
  { key: "cha", name: "Charisma", abbreviation: "CHA", base: 8 },
] as const

type AbilityKey = (typeof abilityDefinitions)[number]["key"]
type AbilitiesOrientation = "horizontal" | "vertical"
type AbilitiesValueOrder = "score-modifier" | "modifier-score"

export function AbilitiesExample() {
  const [scores, setScores] = useState<Record<AbilityKey, number>>(() =>
    abilityDefinitions.reduce(
      (accumulator, ability) => ({
        ...accumulator,
        [ability.key]: ability.base,
      }),
      {} as Record<AbilityKey, number>
    )
  )
  const [orientation, setOrientation] =
    useState<AbilitiesOrientation>("horizontal")
  const [valueOrder, setValueOrder] =
    useState<AbilitiesValueOrder>("score-modifier")

  const abilityRows = useMemo(
    () =>
      abilityDefinitions.map((ability) => ({
        ...ability,
        score: scores[ability.key],
      })),
    [scores]
  )

  const adjustScore = (abilityKey: AbilityKey, amount: number) => {
    setScores((current) => ({
      ...current,
      [abilityKey]: Math.min(30, Math.max(1, current[abilityKey] + amount)),
    }))
  }

  return (
    <>
      <div className="rounded-lg border bg-background p-4">
        <div className="flex flex-wrap items-center justify-end gap-2">
          <div className="flex flex-col gap-2">
            <ToggleGroup
              value={[orientation]}
              onValueChange={(value) => {
                const nextValue = value[0]
                if (nextValue === "horizontal" || nextValue === "vertical") {
                  setOrientation(nextValue)
                }
              }}
              variant="outline"
              spacing={1}
            >
              <ButtonGroup>
                <ToggleGroupItem
                  value="horizontal"
                  render={
                    <Button variant="outline" size="icon">
                      <StretchVerticalIcon className="" />
                    </Button>
                  }
                ></ToggleGroupItem>
                <ToggleGroupItem
                  value="vertical"
                  render={
                    <Button variant="outline" size="icon">
                      <StretchHorizontalIcon className="" />
                    </Button>
                  }
                ></ToggleGroupItem>
              </ButtonGroup>
            </ToggleGroup>
          </div>
          <div className="flex flex-col gap-2">
            <ToggleGroup
              value={[valueOrder]}
              onValueChange={(value) => {
                const nextValue = value[0]
                if (
                  nextValue === "score-modifier" ||
                  nextValue === "modifier-score"
                ) {
                  setValueOrder(nextValue)
                }
              }}
              variant="outline"
              spacing={1}
            >
              <ButtonGroup>
                <ToggleGroupItem
                  value="score-modifier"
                  render={
                    <Button variant="outline" size="icon">
                      <AArrowUpIcon className="" />
                    </Button>
                  }
                ></ToggleGroupItem>
                <ToggleGroupItem
                  value="modifier-score"
                  render={
                    <Button variant="outline" size="icon">
                      <AArrowDownIcon className="" />
                    </Button>
                  }
                ></ToggleGroupItem>
              </ButtonGroup>
            </ToggleGroup>
          </div>
        </div>
      </div>
      <Separator className="my-4" />
      <Card>
        <CardContent className="flex flex-row flex-wrap gap-3">
          <AbilitiesGroup
            orientation={orientation}
            abilities={abilityRows.map((ability) => ({
              name: ability.name,
              abbreviation: ability.abbreviation,
              score: ability.score,
              modifier: getModifier(ability.score),
              onDecreaseBaseScore: () => {
                adjustScore(ability.key, -1)
              },
              onIncreaseBaseScore: () => {
                adjustScore(ability.key, 1)
              },
            }))}
            valueOrder={valueOrder}
          />
        </CardContent>
        <CardFooter className="flex justify-end">
          <span className="text-xs text-muted-foreground">
            Total Points: 27/27
          </span>
        </CardFooter>
      </Card>
    </>
  )
}

function getModifier(score: number) {
  return Math.floor((score - 10) / 2)
}
