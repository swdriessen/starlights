import { ChevronDownIcon, ChevronUpIcon } from "lucide-react"
import { cn } from "ui-framework"
import { Button } from "ui-framework/button"

type AbilitiesItemProps = {
  name: string
  abbreviation: string
  score: number
  modifier: number
  increaseSourceText?: string
  onIncreaseBaseScore: () => void
  onDecreaseBaseScore: () => void
  minScore?: number
  maxScore?: number
  valueOrder?: "score-modifier" | "modifier-score"
  orientation?: "horizontal" | "vertical"
}

type AbilitiesGroupProps = {
  abilities: AbilitiesItemProps[]
  orientation?: "horizontal" | "vertical"
  valueOrder?: "score-modifier" | "modifier-score"
}

export function AbilitiesGroup(props: AbilitiesGroupProps) {
  const {
    abilities,
    orientation = "horizontal",
    valueOrder = "score-modifier",
  } = props

  return (
    <>
      <div
        className={cn(
          "flex flex-wrap gap-4",
          orientation === "vertical" ? "flex-col" : "flex-row"
        )}
      >
        {abilities.map((ability) => (
          <AbilitiesItem
            key={ability.name}
            name={ability.name}
            abbreviation={ability.abbreviation}
            score={ability.score}
            modifier={ability.modifier}
            onIncreaseBaseScore={ability.onIncreaseBaseScore}
            onDecreaseBaseScore={ability.onDecreaseBaseScore}
            minScore={ability.minScore}
            maxScore={ability.maxScore}
            valueOrder={valueOrder}
          />
        ))}
      </div>
    </>
  )
}

export function AbilitiesItem(props: AbilitiesItemProps) {
  const {
    name,
    abbreviation,
    score,
    modifier,
    onIncreaseBaseScore,
    onDecreaseBaseScore,
    valueOrder = "score-modifier",
  } = props

  const nameLayout = true // name-only | abbreviation-only
  const editMode = true
  return (
    <>
      <div className="mb-3 flex gap-2">
        <div className="relative flex items-center">
          <div
            className={`flex h-20 ${nameLayout ? "w-28" : "w-16"} p flex-col items-center justify-center rounded-lg border bg-background text-foreground`}
          >
            <span className="text-xs font-semibold text-muted-foreground uppercase">
              {nameLayout ? name : abbreviation}
            </span>
            <span className="text-xl font-bold">
              {valueOrder === "score-modifier"
                ? score
                : modifier >= 0
                  ? `+${modifier}`
                  : modifier}
            </span>
          </div>
          <div className="absolute -bottom-3 left-1/2 flex h-6 min-w-8 -translate-x-1/2 transform flex-col items-center justify-center rounded-lg border bg-background px-2 text-center text-foreground">
            <span className="text-center text-sm font-semibold">
              {valueOrder === "score-modifier"
                ? modifier >= 0
                  ? `+${modifier}`
                  : modifier
                : score}
            </span>
          </div>
        </div>

        {editMode && (
          <div className="mr-4 flex flex-col justify-between rounded-md border bg-background p-1">
            <Button
              variant="ghost"
              size="icon-sm"
              onClick={() => onIncreaseBaseScore()}
            >
              <ChevronUpIcon />
            </Button>
            {/* <div className="pr-px text-center text-xs font-semibold">
              {score}
            </div> */}
            <Button
              variant="ghost"
              size="icon-sm"
              onClick={() => onDecreaseBaseScore()}
            >
              <ChevronDownIcon />
            </Button>
          </div>
        )}
      </div>
    </>
  )
}
