import { HeartIcon, PlusIcon, SearchIcon, SettingsIcon } from "lucide-react"
import { Button } from "ui-framework"

import { ShowcaseContainer } from "./showcase-container"

const iconButtonVariants = [
  { label: "Default", variant: "default" as const, icon: PlusIcon },
  { label: "Outline", variant: "outline" as const, icon: SearchIcon },
  { label: "Secondary", variant: "secondary" as const, icon: HeartIcon },
  { label: "Ghost", variant: "ghost" as const, icon: SettingsIcon },
]

const iconOnlySizes = [
  { label: "Icon XS", size: "icon-xs" as const },
  { label: "Icon SM", size: "icon-sm" as const },
  { label: "Icon", size: "icon" as const },
  { label: "Icon LG", size: "icon-lg" as const },
]

export function IconButtonShowcase() {
  return (
    <ShowcaseContainer
      title="Icon Button"
      description="Buttons with leading icons and icon-only sizes using lucide icons."
    >
      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-3">
          <h3 className="text-sm font-medium text-foreground">With Icon</h3>
          <div className="flex flex-wrap gap-3">
            {iconButtonVariants.map((iconButtonVariant) => {
              const Icon = iconButtonVariant.icon

              return (
                <Button
                  key={iconButtonVariant.label}
                  variant={iconButtonVariant.variant}
                >
                  <Icon data-icon="inline-start" />
                  {iconButtonVariant.label}
                </Button>
              )
            })}
          </div>
        </div>

        <div className="flex flex-col gap-3">
          <h3 className="text-sm font-medium text-foreground">Icon Only</h3>
          <div className="flex flex-wrap items-center gap-3">
            {iconOnlySizes.map((iconOnlySize) => (
              <Button
                key={iconOnlySize.label}
                size={iconOnlySize.size}
                variant="outline"
                aria-label={iconOnlySize.label}
              >
                <PlusIcon />
              </Button>
            ))}
          </div>
        </div>
      </div>
    </ShowcaseContainer>
  )
}
