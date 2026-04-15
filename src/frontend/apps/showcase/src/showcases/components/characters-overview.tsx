import { HeartIcon } from "lucide-react"
import { Button } from "ui-framework"

type CharacterOverviewItemProps = {
  name: string
  build: string
  portrait: string
  isSelected: boolean
  onSelectedChange: (isSelected: boolean) => void
  isHearted: boolean
  onHeart: () => void
  onView: () => void
  onDelete: () => void
}

export function CharacterOverviewItem({
  name,
  build,
  portrait,
  isSelected,
  onSelectedChange,
  isHearted,
  onHeart,
  onView,
  onDelete,
}: CharacterOverviewItemProps) {
  const isOutlineEnabled = false

  return (
    <div className="w-52 max-w-full min-w-50 rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div
        className={`group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md ${isSelected && isOutlineEnabled ? "outline-2 outline-offset-2 outline-emerald-600/90 dark:outline-emerald-400/90" : "outline-none"}`}
        role="button"
        tabIndex={0}
        aria-pressed={isSelected}
        onClick={() => onSelectedChange(!isSelected)}
        onKeyDown={(event) => {
          if (event.key === "Enter" || event.key === " ") {
            event.preventDefault()
            onSelectedChange(!isSelected)
          }
        }}
      >
        <div className="block">
          <img
            src={portrait}
            alt={`Portrait of ${name}`}
            className="aspect-square w-full object-cover grayscale-20 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </div>
        <div
          className={`absolute top-2 right-2 transition-all duration-300 ${isHearted || isSelected ? "pointer-events-auto translate-y-0 opacity-100" : "pointer-events-none -translate-y-2 opacity-0 group-hover:pointer-events-auto group-hover:translate-y-0 group-hover:opacity-100"}`}
        >
          <Button
            size="icon"
            variant="ghost"
            className=""
            aria-pressed={isHearted}
            onClick={(event) => {
              event.stopPropagation()
              onHeart()
            }}
          >
            <HeartIcon
              className={`size-5 transition-colors duration-200 ${isHearted ? "fill-red-500 text-red-500" : "fill-transparent text-red-400"}`}
            />
          </Button>
        </div>
        <div className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white">
          <div className={`transition-transform duration-300`}>
            <p
              className={`truncate text-sm font-semibold tracking-widest uppercase transition-colors duration-200`}
            >
              {name}
            </p>
            <p className="text-xs text-white/90">{build}</p>
          </div>
          <div
            className={`flex gap-2 overflow-hidden transition-all duration-300 ${isSelected ? "mt-2 max-h-10 opacity-100" : "mt-0 max-h-0 opacity-0 group-hover:mt-2 group-hover:max-h-10 group-hover:opacity-100"}`}
          >
            <Button
              size="sm"
              variant="outline"
              className="flex-1 border-zinc-600 bg-zinc-900/90 text-zinc-100 hover:bg-zinc-800/95 hover:text-zinc-50"
              onClick={(event) => {
                event.stopPropagation()
                onView()
              }}
            >
              Manage
            </Button>
            <Button
              size="sm"
              variant="destructive"
              className="flex-1 border-red-500 bg-red-600/95 text-white hover:bg-red-500"
              onClick={(event) => {
                event.stopPropagation()
                onDelete()
              }}
            >
              Delete
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
