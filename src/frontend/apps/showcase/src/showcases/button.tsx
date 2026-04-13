import { Button } from "ui-framework"

import { ShowcaseContainer } from "./showcase-container"

const buttonVariants = [
  { label: "Default", variant: "default" as const },
  { label: "Outline", variant: "outline" as const },
  { label: "Secondary", variant: "secondary" as const },
  { label: "Ghost", variant: "ghost" as const },
  { label: "Destructive", variant: "destructive" as const },
  { label: "Link", variant: "link" as const },
]

const buttonSizes = [
  { label: "Extra Small", size: "xs" as const },
  { label: "Small", size: "sm" as const },
  { label: "Default", size: "default" as const },
  { label: "Large", size: "lg" as const },
]

export function ButtonShowcase() {
  return (
    <ShowcaseContainer
      title="Button"
      description="Variants and sizes for the primary action component."
    >
      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-3">
          <h3 className="text-sm font-medium text-foreground">Variants</h3>
          <div className="flex flex-wrap gap-3">
            {buttonVariants.map((buttonVariant) => (
              <Button key={buttonVariant.label} variant={buttonVariant.variant}>
                {buttonVariant.label}
              </Button>
            ))}
          </div>
        </div>

        <div className="flex flex-col gap-3">
          <h3 className="text-sm font-medium text-foreground">Sizes</h3>
          <div className="flex flex-wrap items-center gap-3">
            {buttonSizes.map((buttonSize) => (
              <Button key={buttonSize.label} size={buttonSize.size}>
                {buttonSize.label}
              </Button>
            ))}
          </div>
        </div>
      </div>
    </ShowcaseContainer>
  )
}
