import { AnvilIcon, HeartIcon, SwordsIcon, Trash2Icon } from "lucide-react"

import { useEffect, useRef } from "react"
import { Button } from "@/components/ui/button"
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip"
import { cn } from "@/lib/utils"
import {
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction,
  AlertDialog,
} from "@/components/ui/alert-dialog"

type CharacterGridItemProps = {
  id: string
  name: string
  build: string
  portrait: string
  isSelected: boolean
  onSelectedChange: (id: string, isSelected: boolean) => void
  isHearted: boolean
  onHeartToggle: (id: string) => void
  onManage: (id: string) => void
  onViewSheet: (id: string) => void
  onDelete: (id: string) => void
}

export function CharacterGrid({
  children,
  className,
}: {
  children: React.ReactNode
  className?: string
}) {
  return <div className={cn("flex flex-wrap gap-8", className)}>{children}</div>
}

export function CharacterGridItem({
  id,
  name,
  build,
  portrait,
  isSelected,
  onSelectedChange,
  isHearted,
  onHeartToggle,
  onManage,
  onViewSheet,
  onDelete,
}: CharacterGridItemProps) {
  const rootRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    if (!isSelected) return

    function handleOutside(e: Event) {
      if (rootRef.current && !rootRef.current.contains(e.target as Node)) {
        onSelectedChange(id, false)
      }
    }

    document.addEventListener("mousedown", handleOutside)
    document.addEventListener("touchstart", handleOutside)

    return () => {
      document.removeEventListener("mousedown", handleOutside)
      document.removeEventListener("touchstart", handleOutside)
    }
  }, [isSelected, id, onSelectedChange])

  return (
    // rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60
    <div ref={rootRef} className="w-52 max-w-full min-w-50 p-0.5">
      <div className="group relative overflow-hidden rounded-xl border-4 border-double border-border bg-card ring-border/60 transition-shadow hover:shadow-lg">
        <button
          type="button"
          className="block w-full cursor-pointer text-left focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-emerald-600/90"
          aria-pressed={isSelected}
          aria-label={
            isSelected ? `Hide actions for ${name}` : `Show actions for ${name}`
          }
          onClick={() => onSelectedChange(id, !isSelected)}
        >
          <img
            src={portrait}
            alt={`Portrait of ${name}`}
            className="aspect-square w-full object-cover grayscale-20 transition duration-500 group-hover:scale-115 group-hover:grayscale-0"
          />
        </button>
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
              onHeartToggle(id)
            }}
          >
            <HeartIcon
              className={`size-5 transition-colors duration-200 ${isHearted ? "fill-red-500 text-red-500" : "fill-transparent text-red-400"}`}
            />
          </Button>
        </div>
        <div className="pointer-events-none absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white">
          <div className={`transition-transform duration-300`}>
            <p
              className={`truncate text-sm font-semibold tracking-widest uppercase transition-colors duration-200`}
            >
              {name}
            </p>
            <p className="text-xs text-white/90">{build}</p>
          </div>
          <div
            className={`pointer-events-auto flex gap-2 overflow-hidden transition-all duration-300 ${isSelected ? "mt-2 max-h-10 opacity-100" : "mt-0 max-h-0 opacity-0 group-hover:mt-2 group-hover:max-h-10 group-hover:opacity-100"}`}
          >
            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    size="sm"
                    variant="outline"
                    className="flex-1 border-zinc-600 bg-zinc-900/90 text-zinc-100 hover:bg-zinc-800/95 hover:text-zinc-50"
                    onClick={(event) => {
                      event.stopPropagation()
                      onViewSheet(id)
                    }}
                  />
                }
              >
                <SwordsIcon className="mr-1 size-4" />
                Sheet
              </TooltipTrigger>
              <TooltipContent side="top">View Character Sheet</TooltipContent>
            </Tooltip>

            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    size="icon-sm"
                    variant="outline"
                    aria-label="Manage Build"
                    className="border-zinc-600 bg-zinc-900/90 text-zinc-100 hover:bg-zinc-800/95 hover:text-zinc-50"
                    onClick={(event) => {
                      event.stopPropagation()
                      onManage(id)
                    }}
                  />
                }
              >
                <AnvilIcon className="size-4" />
              </TooltipTrigger>
              <TooltipContent side="top">Manage Build</TooltipContent>
            </Tooltip>

            <AlertDialog>
              <Tooltip>
                <TooltipTrigger
                  render={
                    <AlertDialogTrigger
                      render={
                        <Button
                          size="icon-sm"
                          variant="destructive"
                          aria-label="Delete Character"
                          className="border-red-500 bg-red-600/95 text-white hover:bg-red-500"
                        />
                      }
                    />
                  }
                >
                  <Trash2Icon className="size-4" />
                </TooltipTrigger>
                <TooltipContent side="top">Delete Character</TooltipContent>
              </Tooltip>

              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Delete Character</AlertDialogTitle>
                  <AlertDialogDescription>
                    Are you sure you want to delete <strong>{name}</strong>?
                    This action cannot be undone.
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel variant="outline" size="default">
                    Cancel
                  </AlertDialogCancel>
                  <AlertDialogAction
                    variant="destructive"
                    size="default"
                    onClick={() => {
                      onDelete(id)
                    }}
                  >
                    Delete
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </div>
        </div>
      </div>
    </div>
  )
}
