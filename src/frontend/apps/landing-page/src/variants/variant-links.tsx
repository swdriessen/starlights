import { variants, type VariantId } from "./variant-list"

/** Footer switcher shown on every landing variant. Plain links: each variant is its own page. */
export function VariantLinks({
  current,
  className,
  linkClass,
  activeClass,
}: {
  current: VariantId
  className?: string
  linkClass?: string
  activeClass?: string
}) {
  return (
    <nav aria-label="Landing page variants" className={className}>
      <ul className="flex flex-wrap items-center gap-x-5 gap-y-2">
        {variants.map((variant) => (
          <li key={variant.id}>
            <a
              href={variant.path}
              aria-current={variant.id === current ? "page" : undefined}
              className={
                variant.id === current
                  ? `${linkClass ?? ""} ${activeClass ?? ""}`
                  : linkClass
              }
            >
              <span className="font-semibold uppercase">{variant.id}</span>
              <span className="ml-1.5 opacity-70">{variant.name}</span>
            </a>
          </li>
        ))}
      </ul>
    </nav>
  )
}
