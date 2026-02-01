"use client";

import { InputGroup, InputGroupAddon, InputGroupButton, InputGroupInput } from "@/components/ui/input-group";
import { cn } from "@/lib/utils";
import { Autocomplete as AutocompletePrimitive } from "@base-ui/react/autocomplete";
import { ChevronDownIcon, XIcon } from "lucide-react";
import { createContext, useContext, useImperativeHandle, useRef, type ComponentProps, type RefObject } from "react";

export type AutocompleteProps = ComponentProps<typeof AutocompletePrimitive.Root>;

const AutocompleteAnchorContext = createContext<{
  anchorRef: RefObject<HTMLInputElement | null>;
} | null>(null);

const useAnchorRef = () => {
  const context = useContext(AutocompleteAnchorContext);
  if (!context) {
    throw new Error("Autocomplete components must be used within <Autocomplete>.");
  }
  return context;
};

function Autocomplete({ children, ...props }: AutocompleteProps) {
  const anchorRef = useRef<HTMLInputElement | null>(null);
  return (
    <AutocompletePrimitive.Root {...props}>
      <AutocompleteAnchorContext.Provider value={{ anchorRef }}>{children}</AutocompleteAnchorContext.Provider>
    </AutocompletePrimitive.Root>
  );
}

function AutocompleteInput({ children, ref, ...props }: ComponentProps<typeof AutocompletePrimitive.Input>) {
  const { anchorRef } = useAnchorRef();
  useImperativeHandle(ref, () => anchorRef.current!, [anchorRef]);
  return (
    <InputGroup ref={anchorRef}>
      <AutocompletePrimitive.Input render={InputGroupInput} {...props} />
      {children}
    </InputGroup>
  );
}

function AutocompleteClear({ ...props }: ComponentProps<typeof AutocompletePrimitive.Clear>) {
  return (
    <InputGroupAddon align="inline-end">
      <AutocompletePrimitive.Clear render={(props) => <InputGroupButton size="icon-xs" {...props} />} {...props}>
        <XIcon />
      </AutocompletePrimitive.Clear>
    </InputGroupAddon>
  );
}

function AutocompleteTrigger({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.Trigger>) {
  return (
    <InputGroupAddon align="inline-end">
      <AutocompletePrimitive.Trigger
        render={(props) => <InputGroupButton size="icon-xs" {...props} />}
        className={cn("data-popup-open:[&_svg]:rotate-180 [&_svg]:transition-transform", className)}
        {...props}
      >
        <AutocompleteIcon render={(props) => <ChevronDownIcon {...props} />} />
      </AutocompletePrimitive.Trigger>
    </InputGroupAddon>
  );
}

function AutocompleteValue({ ...props }: ComponentProps<typeof AutocompletePrimitive.Value>) {
  return <AutocompletePrimitive.Value {...props} />;
}

function AutocompleteIcon({ ...props }: ComponentProps<typeof AutocompletePrimitive.Icon>) {
  return <AutocompletePrimitive.Icon {...props} />;
}

function AutocompletePopup({ className, children, ...props }: ComponentProps<typeof AutocompletePrimitive.Popup>) {
  const { anchorRef } = useAnchorRef();
  return (
    <AutocompletePrimitive.Portal>
      <AutocompletePrimitive.Positioner
        anchor={anchorRef}
        className="data-[side=bottom]:translate-y-1 data-[side=left]:-translate-x-1 data-[side=right]:translate-x-1 data-[side=top]:-translate-y-1"
      >
        <AutocompletePrimitive.Popup
          className={cn(
            "px-1 bg-popover text-popover-foreground rounded-md border shadow-md",
            "w-(--anchor-width) max-w-(--available-width) max-h-92 overflow-hidden",
            "data-open:animate-in data-closed:animate-out data-closed:fade-out-0 data-open:fade-in-0 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2",
            className,
          )}
          {...props}
        >
          {children}
        </AutocompletePrimitive.Popup>
      </AutocompletePrimitive.Positioner>
    </AutocompletePrimitive.Portal>
  );
}

function AutocompleteEmpty({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.Empty>) {
  return <AutocompletePrimitive.Empty className={cn("py-2 pl-2 empty:p-0 text-sm text-muted-foreground", className)} {...props} />;
}

function AutocompleteList({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.List>) {
  return (
    <AutocompletePrimitive.List
      className={cn("py-1 overflow-y-auto scroll-py-2 overscroll-contain max-h-[min(23rem,var(--available-height))] data-empty:p-0", className)}
      {...props}
    />
  );
}

function AutocompleteItem({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.Item>) {
  return (
    <AutocompletePrimitive.Item
      className={cn(
        "cursor-default flex items-center gap-2 py-1 pl-2 pr-8 text-sm outline-hidden select-none",
        "data-highlighted:relative data-highlighted:z-0 data-highlighted:text-accent-foreground data-highlighted:before:absolute data-highlighted:before:inset-0 data-highlighted:before:z-[-1] data-highlighted:before:rounded-sm data-highlighted:before:bg-accent",
        "data-disabled:pointer-events-none data-disabled:opacity-50",
        className,
      )}
      {...props}
    />
  );
}

function AutocompleteRow({ ...props }: ComponentProps<typeof AutocompletePrimitive.Row>) {
  return <AutocompletePrimitive.Row {...props} />;
}

function AutocompleteCollection({ ...props }: ComponentProps<typeof AutocompletePrimitive.Collection>) {
  return <AutocompletePrimitive.Collection {...props} />;
}

function AutocompleteGroup({ ...props }: ComponentProps<typeof AutocompletePrimitive.Group>) {
  return <AutocompletePrimitive.Group {...props} />;
}

function AutocompleteGroupLabel({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.GroupLabel>) {
  return <AutocompletePrimitive.GroupLabel className={cn("text-muted-foreground px-2 py-1.5 text-xs", className)} {...props} />;
}

function AutocompleteSeparator({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.Separator>) {
  return <AutocompletePrimitive.Separator className={cn("bg-border pointer-events-none my-1 h-px", className)} {...props} />;
}

function AutocompleteStatus({ className, ...props }: ComponentProps<typeof AutocompletePrimitive.Status>) {
  return <AutocompletePrimitive.Status className={cn("py-2 pl-2 empty:p-0 text-sm text-muted-foreground", className)} {...props} />;
}

export {
  Autocomplete,
  AutocompleteClear,
  AutocompleteCollection,
  AutocompleteEmpty,
  AutocompleteGroup,
  AutocompleteGroupLabel,
  AutocompleteIcon,
  AutocompleteInput,
  AutocompleteItem,
  AutocompleteList,
  AutocompletePopup,
  AutocompleteRow,
  AutocompleteSeparator,
  AutocompleteStatus,
  AutocompleteTrigger,
  AutocompleteValue,
};
