import * as React from "react";
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";

const pageContentVariants = cva("px-0 sm:px-0 ", {
  variants: {
    variant: {
      default: "",
      borderless: "",
      wide: "",
      noPadding: "",
    },
  },
  defaultVariants: {
    variant: "default",
  },
});

const pageContentContainerVariants = cva("", {
  variants: {
    variant: {
      default: "container mx-auto bg-background sm:rounded-xl sm:border border shadow-lg sm:mt-12 p-8",
      borderless: "container mx-auto sm:mt-12 px-0 sm:px-0",
      wide: " ",
      noPadding: "container mx-auto  sm:rounded-xl shadow-lg sm:mt-12",
    },
  },
  defaultVariants: {
    variant: "default",
  },
});

const pageContentMainVariants = cva("", {
  variants: {
    variant: {
      default: "p-0",
      borderless: "p-0",
      wide: "",
      noPadding: " p-0",
    },
  },
  defaultVariants: {
    variant: "default",
  },
});

export function PageContent({
  children,
  variant,
  className,
}: {
  children: React.ReactNode;
} & VariantProps<typeof pageContentVariants> & {
    className?: string;
  }) {
  const showMain = variant !== "wide";

  return (
    <div className={cn(pageContentVariants({ variant }))}>
      <div className={cn(pageContentContainerVariants({ variant }), className)}>
        {showMain ? <main className={pageContentMainVariants({ variant })}>{children}</main> : children}
      </div>
    </div>
  );
}
