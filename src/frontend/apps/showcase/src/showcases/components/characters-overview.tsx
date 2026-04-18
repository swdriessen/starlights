import {
  AnvilIcon,
  HeartIcon,
  SwordsIcon,
  Trash2Icon,
} from "lucide-react"
import { Button, Tooltip, TooltipContent, TooltipTrigger } from "ui-framework"

type CollectionItemProps = {
  id: string
  name: string
  build: string
  portrait: string
  isSelected: boolean
  onSelectedChange: (id: string, isSelected: boolean) => void
  isHearted: boolean
  onHeartToggle: (id: string) => void
  onManage: (id: string) => void
  onDelete: (id: string) => void
}

export function CollectionContainer({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <>
      <div className="flex flex-wrap gap-8">{children}</div>
    </>
  )
}

export function CollectionItem({
  id,
  name,
  build,
  portrait,
  isSelected,
  onSelectedChange,
  isHearted,
  onHeartToggle,
  onManage,
  onDelete,
}: CollectionItemProps) {
  return (
    <div className="w-52 max-w-full min-w-50 rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div className="group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md">
        <button
          type="button"
          className="block w-full cursor-pointer text-left focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-emerald-600/90"
          aria-pressed={isSelected}
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
                      onManage(id)
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

            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    size="icon-sm"
                    variant="destructive"
                    aria-label="Delete Character"
                    className="border-red-500 bg-red-600/95 text-white hover:bg-red-500"
                    onClick={(event) => {
                      event.stopPropagation()
                      onDelete(id)
                    }}
                  />
                }
              >
                <Trash2Icon className="size-4" />
              </TooltipTrigger>
              <TooltipContent side="top">Delete Character</TooltipContent>
            </Tooltip>

            {/* <AlertDialog>
              <AlertDialogTrigger render={<Button variant="outline" />}>
                <span className="hidden md:block">Alert Dialog</span>
                <span className="block md:hidden">Dialog</span>
              </AlertDialogTrigger>
              <AlertDialogContent size="sm">
                <AlertDialogHeader>
                  <AlertDialogTitle>
                    Allow accessory to connect?
                  </AlertDialogTitle>
                  <AlertDialogDescription>
                    Do you want to allow the USB accessory to connect to this
                    device and your data?
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel variant="outline" size="default">
                    Don&apos;t allow
                  </AlertDialogCancel>
                  <AlertDialogAction>Allow</AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog> */}

            {/* <DropdownMenu>
              <DropdownMenuTrigger
                render={
                  <Button
                    variant="outline"
                    className="border-zinc-600 bg-zinc-900/90 text-zinc-100 hover:bg-zinc-800/95 hover:text-zinc-50"
                    size="icon-sm"
                  />
                }
              >
                <MoreHorizontalIcon />
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" side="top" className="w-40">
                <DropdownMenuGroup>
                  <DropdownMenuLabel>Quick Actions</DropdownMenuLabel>
                  <DropdownMenuItem>Level Up</DropdownMenuItem>
                  <DropdownMenuItem>Duplicate Character</DropdownMenuItem>
                  <DropdownMenuItem>Change Group</DropdownMenuItem>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuLabel>Campaigns</DropdownMenuLabel>
                  <DropdownMenuItem>Show Campaign</DropdownMenuItem>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuItem variant="destructive">
                    Delete Character
                  </DropdownMenuItem>
                </DropdownMenuGroup>
              </DropdownMenuContent>
            </DropdownMenu> */}
          </div>
        </div>
      </div>
    </div>
  )
}
