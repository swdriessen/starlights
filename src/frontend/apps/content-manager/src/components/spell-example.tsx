"use client";

import * as React from "react";

import { Example, ExampleWrapper } from "@/components/example";

import { Button } from "@/components/ui/button";
import { Card, CardAction, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Combobox,
  ComboboxChip,
  ComboboxChips,
  ComboboxChipsInput,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
  ComboboxValue,
  useComboboxAnchor,
} from "@/components/ui/combobox";
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuPortal,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuSeparator,
  DropdownMenuShortcut,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Field,
  FieldContent,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldLegend,
  FieldSeparator,
  FieldSet,
  FieldTitle,
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import {
  PlusIcon,
  BluetoothIcon,
  MoreVerticalIcon,
  FileIcon,
  FolderIcon,
  FolderOpenIcon,
  FileCodeIcon,
  MoreHorizontalIcon,
  FolderSearchIcon,
  SaveIcon,
  DownloadIcon,
  EyeIcon,
  LayoutIcon,
  PaletteIcon,
  SunIcon,
  MoonIcon,
  MonitorIcon,
  UserIcon,
  CreditCardIcon,
  SettingsIcon,
  KeyboardIcon,
  LanguagesIcon,
  BellIcon,
  MailIcon,
  ShieldIcon,
  HelpCircleIcon,
  FileTextIcon,
  LogOutIcon,
  MailQuestionIcon,
  CircleQuestionMarkIcon,
  IndentIcon,
} from "lucide-react";
import { Tooltip, TooltipContent, TooltipTrigger } from "./ui/tooltip";
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar";
import { Checkbox } from "./ui/checkbox";
import { Separator } from "./ui/separator";
import { RadioGroup, RadioGroupItem } from "./ui/radio-group";
import { InputGroup, InputGroupAddon, InputGroupButton, InputGroupInput } from "./ui/input-group";
import { useState } from "react";
import {
  Autocomplete,
  AutocompleteClear,
  AutocompleteEmpty,
  AutocompleteInput,
  AutocompleteItem,
  AutocompleteList,
  AutocompletePopup,
  AutocompleteTrigger,
} from "@/components/custom/autocomplete";
import { Alert, AlertDescription } from "./ui/alert";
import { Toggle } from "./ui/toggle";
import { Popover, PopoverContent, PopoverDescription, PopoverHeader, PopoverTitle, PopoverTrigger } from "./ui/popover";

import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "./ui/accordion";
import { cn } from "@/lib/utils";

import Markdown from "react-markdown";
import remarkGfm from "remark-gfm";
import TableComponent from "./tabs";

export function SpellComponentExample() {
  const spelllevels = [
    { label: "Cantrip", value: "0" },
    { label: "1st", value: "1" },
    { label: "2nd", value: "2" },
    { label: "3rd", value: "3" },
    { label: "4th", value: "4" },
    { label: "5th", value: "5" },
    { label: "6th", value: "6" },
    { label: "7th", value: "7" },
    { label: "8th", value: "8" },
    { label: "9th", value: "9" },
  ];

  const schools = [
    // { label: "x", value: null },
    { label: "Abjuration", value: "abjuration" },
    { label: "Conjuration", value: "conjuration" },
    { label: "Divination", value: "divination" },
    { label: "Enchantment", value: "enchantment" },
    { label: "Evocation", value: "evocation" },
    { label: "Illusion", value: "illusion" },
    { label: "Necromancy", value: "necromancy" },
    { label: "Transmutation", value: "transmutation" },
  ];

  const times = [
    { label: "Action", value: "Action" },
    { label: "Bonus Action", value: "Bonus Action" },
    { label: "Reaction", value: "Reaction" },
    { label: "1 Minute", value: "1 Minute" },
    { label: "10 Minutes", value: "10 Minutes" },
    { label: "1 Hour", value: "1 Hour" },
    { label: "8 Hours", value: "8 Hours" },
    { label: "12 Hours", value: "12 Hours" },
    { label: "24 Hours", value: "24 Hours" },
  ];
  return (
    <>
      <div className="container mx-auto mt-12 mb-100">
        <div className="mb-4">
          <Card>
            <CardContent className=" flex items-center justify-between gap-6">
              <FieldTitle>Manage Spell</FieldTitle>
              <div className="flex gap-2">
                <Button
                  variant="default"
                  form="manage-spell"
                >
                  Save Changes
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
        <div className="grid grid-cols-12 gap-4">
          <div className="col-span-7 flex flex-col gap-4">
            <Card>
              <CardContent className=" flex flex-col gap-6">
                {/* name */}
                <FieldSet>
                  <FieldGroup>
                    <Field>
                      <FieldLabel htmlFor="input-name">Name</FieldLabel>
                      <Input
                        id="input-name"
                        placeholder="Fireball"
                      />
                      {/* <FieldDescription>The name of the spell.</FieldDescription> */}
                    </Field>
                  </FieldGroup>
                </FieldSet>

                {/* spell details */}
                <FieldSet>
                  <FieldGroup>
                    <div className="grid grid-cols-12 gap-4">
                      <Field className="col-span-4">
                        <FieldLabel htmlFor="checkout-7j9-exp-month-ts6">Level</FieldLabel>
                        <Select
                          items={spelllevels}
                          defaultValue={"1"}
                        >
                          <SelectTrigger id="checkout-7j9-exp-month-ts6">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectGroup>
                              {spelllevels.map((item) => (
                                <SelectItem
                                  key={item.value}
                                  value={item.value}
                                >
                                  {item.label}
                                </SelectItem>
                              ))}
                            </SelectGroup>
                          </SelectContent>
                        </Select>
                      </Field>

                      <Field className="col-span-8">
                        <FieldLabel htmlFor="checkout-7j9-exp-year-f59">
                          School of Magic
                          <InfoPopover description="These categories help describe spells but have no rules of their own, although some other rules refer to them." />
                        </FieldLabel>
                        <Select
                          items={schools}
                          defaultValue={"Abjuration"}
                        >
                          <SelectTrigger id="checkout-7j9-exp-year-f59">
                            <SelectValue />
                          </SelectTrigger>
                          <SelectContent>
                            <SelectGroup>
                              {schools.map((item) => (
                                <SelectItem
                                  key={item.value}
                                  value={item.value}
                                >
                                  {item.label}
                                </SelectItem>
                              ))}
                            </SelectGroup>
                          </SelectContent>
                        </Select>
                        {/* <FieldDescription>
                        These categories help describe spells but have no rules of their own, although some other rules refer to them.
                      </FieldDescription> */}
                      </Field>
                    </div>
                  </FieldGroup>
                </FieldSet>

                {/* <FieldSeparator /> */}

                {/* casting time */}
                <FieldSet>
                  {/* <FieldLegend>Casting Time</FieldLegend>
                  <FieldDescription>The time required to cast the spell.</FieldDescription> */}
                  <FieldGroup>
                    <Field>
                      <FieldLabel htmlFor="input-casting-time">
                        Casting Time
                        {/* <InfoPopover description="A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more." /> */}
                      </FieldLabel>
                      {/* <FieldDescription>
                      A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more.
                    </FieldDescription> */}

                      <Autocomplete
                        id="input-casting-time"
                        items={times}
                      >
                        <AutocompleteInput placeholder="Action">
                          <AutocompleteClear />
                          <AutocompleteTrigger />
                        </AutocompleteInput>
                        <AutocompletePopup>
                          <AutocompleteEmpty>No items found</AutocompleteEmpty>
                          <AutocompleteList>
                            {(item: { label: string; value: string }) => (
                              <AutocompleteItem
                                key={item.value}
                                value={item.value}
                              >
                                {item.label}
                              </AutocompleteItem>
                            )}
                          </AutocompleteList>
                        </AutocompletePopup>
                      </Autocomplete>

                      {/* <FieldDescription>The time required to cast the spell.</FieldDescription> */}
                    </Field>

                    {/* <FieldSeparator /> */}
                    <Field orientation="horizontal">
                      <Checkbox id="is-ritual-spell" />
                      <FieldContent>
                        <FieldLabel
                          htmlFor="is-ritual-spell"
                          className="font-normal"
                        >
                          Ritual Spell
                        </FieldLabel>
                        <FieldDescription>Indicates whether the spell can be cast as a ritual.</FieldDescription>
                      </FieldContent>
                    </Field>
                  </FieldGroup>
                </FieldSet>

                {/* <FieldSeparator /> */}

                {/* range */}
                <RangeFieldGroup />

                {/* <FieldSeparator /> */}

                {/* duration */}
                <DurationExample />
              </CardContent>
            </Card>
            <Card>
              <CardContent className=" flex flex-col gap-4">
                <SpellComponentsWithToggles />
              </CardContent>
            </Card>
            <Card>
              <CardContent className=" flex flex-col gap-4">
                <DescriptionComp />
              </CardContent>
            </Card>
            {/* <div className="w-full">
              <TableComponent />
            </div> */}
          </div>
          <div className="col-span-5 flex flex-col gap-4">
            {/* <ContributionsActivity /> */}
            {/* <Card>
              <CardContent className=" flex flex-col gap-4">
                <SpellLevelExample />
              </CardContent>
            </Card> */}
            <Card>
              <CardHeader className="">
                <CardTitle>Properties</CardTitle>
                <CardDescription>Manage your spell properties.</CardDescription>
              </CardHeader>
              <CardContent className=" flex flex-col gap-4">
                <FieldGroup>
                  <FieldSet>
                    <FieldLegend className="sr-only">Contributions & activity</FieldLegend>
                    <FieldGroup className="gap-3">
                      {/* <Field orientation="horizontal">
                        <Checkbox id="private-profile" />
                        <FieldContent>
                          <FieldLabel
                            htmlFor="private-profile"
                            className="font-normal"
                          >
                            Make profile private and hide activity
                          </FieldLabel>
                          <FieldDescription>
                            Enabling this will hide your contributions and activity from your GitHub profile and from social features like
                            followers, stars, feeds, leaderboards and releases.
                          </FieldDescription>
                        </FieldContent>
                      </Field> */}
                      {/* <Field orientation="horizontal">
                        <Checkbox
                          id="private-contributions"
                          defaultChecked
                        />
                        <FieldContent>
                          <FieldLabel
                            htmlFor="private-contributions"
                            className="font-normal"
                          >
                            Include private contributions on my profile
                          </FieldLabel>
                          <FieldDescription>
                            Your contribution graph, achievements, and activity overview will show your private contributions without
                            revealing any repository or organization information. <a href="#read-more">Read more</a>.
                          </FieldDescription>
                        </FieldContent>
                      </Field> */}

                      <Field orientation="horizontal">
                        <Checkbox id="is-ritual-spell" />
                        <FieldContent>
                          <FieldLabel
                            htmlFor="is-ritual-spell"
                            className="font-normal"
                          >
                            Ritual Spell
                          </FieldLabel>
                          <FieldDescription>Indicates whether the spell can be cast as a ritual.</FieldDescription>
                        </FieldContent>
                      </Field>

                      <Field orientation="horizontal">
                        <Checkbox id="is-concentration-spell" />
                        <FieldContent>
                          <FieldLabel
                            htmlFor="is-concentration-spell"
                            className="font-normal"
                          >
                            Concentration Spell
                          </FieldLabel>
                          <FieldDescription>Indicates whether the spell requires concentration.</FieldDescription>
                        </FieldContent>
                      </Field>
                    </FieldGroup>
                  </FieldSet>
                </FieldGroup>
              </CardContent>
            </Card>
            <Card>
              <CardContent className=" flex flex-col gap-4">
                <SpellComponentsWithToggles />
              </CardContent>
            </Card>
            {/* <AssignClasses /> */}{" "}
          </div>

          {/* <div className="col-span-12 flex flex-col gap-4">
            <Card>
              <CardContent className=" flex flex-col gap-4">
                <DescriptionComp />
              </CardContent>
            </Card>
          </div> */}
        </div>
      </div>
    </>
  );
}

const frameworks = ["Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro"] as const;

const roleItems = [
  { label: "Developer", value: "developer" },
  { label: "Designer", value: "designer" },
  { label: "Manager", value: "manager" },
  { label: "Other", value: "other" },
];

function SpellComponentsWithToggles() {
  return (
    <form id="spell-components-form">
      <FieldGroup>
        <FieldSet>
          <FieldGroup>
            <Field orientation="vertical">
              <FieldContent>
                <FieldLabel htmlFor="terms-1">Spell Components</FieldLabel>
                <FieldDescription>
                  A spell's components are physical requirements the spellcaster must meet to cast the spell.
                </FieldDescription>
              </FieldContent>

              <div className="flex items-center gap-2 mt-2">
                <Tooltip>
                  <TooltipTrigger
                    render={
                      <Toggle
                        variant="outline"
                        size="sm"
                        className="data-pressed:bg-primary data-pressed:text-primary-foreground uppercase transition-colors"
                      />
                    }
                  >
                    V
                  </TooltipTrigger>
                  <TooltipContent sideOffset={10}>Verbal Component</TooltipContent>
                </Tooltip>

                <Tooltip>
                  <TooltipTrigger
                    render={
                      <Toggle
                        variant="outline"
                        size="sm"
                        className="data-pressed:bg-primary data-pressed:text-primary-foreground uppercase transition-colors"
                      />
                    }
                  >
                    S
                  </TooltipTrigger>
                  <TooltipContent sideOffset={10}>Somatic Component</TooltipContent>
                </Tooltip>

                <Tooltip>
                  <TooltipTrigger
                    render={
                      <Toggle
                        variant="outline"
                        size="sm"
                        className="data-pressed:bg-primary data-pressed:text-primary-foreground uppercase transition-colors"
                      />
                    }
                  >
                    M
                  </TooltipTrigger>
                  <TooltipContent sideOffset={10}>Material Component</TooltipContent>
                </Tooltip>
              </div>
            </Field>

            <Field>
              <FieldLabel htmlFor="material-component-description">Material Component Description</FieldLabel>
              <Textarea
                id="material-component-description"
                placeholder="a bit of flour"
                className="min-h-8 py-1.5"
              />
            </Field>
          </FieldGroup>
        </FieldSet>
      </FieldGroup>
    </form>
  );
}

function DescriptionComp() {
  const [description, setDescription] = React.useState("test markdown");
  return (
    <>
      {/* <CardHeader className="border-b">
          <CardTitle>Spell Components</CardTitle>
          <CardDescription>A spell's components are physical requirements the spellcaster must meet to cast the spell.</CardDescription>
        </CardHeader> */}

      <form id="spell-components-form">
        <FieldGroup>
          <FieldSet>
            <FieldGroup>
              {/* <Field className="">
                    <FieldContent>
                      <FieldLabel htmlFor="terms-1">Description</FieldLabel>
                      <FieldDescription>The description of the spell should only contain</FieldDescription>
                    </FieldContent>
                  </Field> */}

              <Field className="">
                <FieldLabel htmlFor="material-component-description">Description</FieldLabel>
                <Textarea
                  id="material-component-description"
                  className="min-h-50"
                  placeholder="You create an acidic bubble at a point within range, where it explodes in a 5-foot-radius Sphere. Each creature in that Sphere must succeed on a Dexterity saving throw or take 1d6 Acid damage."
                  onChange={(e) => {
                    setDescription(e.target.value);
                  }}
                />
              </Field>

              {/* <Field className="">
                    <FieldLabel htmlFor="material-component-description">
                      Using a Higher-Level Spell Slot
                    </FieldLabel>
                    <Textarea
                      id="material-component-description"
                      placeholder="Each target's Hit Points increase by 5 for each spell slot level above 2."
                    />
                  </Field> */}

              {/* <Field className="">
                    <FieldLabel htmlFor="material-component-description">
                      Cantrip Upgrade
                    </FieldLabel>
                    <Textarea
                      id="material-component-description"
                      placeholder="The damage increases by 1d6 when you reach levels 5 (2d6),11 (3d6), and 17 (4d6)."
                    />
                  </Field> */}
            </FieldGroup>
          </FieldSet>
        </FieldGroup>
      </form>
    </>
  );
}

function RangeFieldGroup() {
  const times = [
    { label: "Touch", value: "Touch" },
    { label: "Self", value: "Self" },
    { label: "10 feet", value: "10 feet" },
    { label: "30 feet", value: "30 feet" },
    { label: "60 feet", value: "60 feet" },
    { label: "90 feet", value: "90 feet" },
    { label: "120 feet", value: "120 feet" },
    { label: "150 feet", value: "150 feet" },
    { label: "300 feet", value: "300 feet" },
  ];

  return (
    <FieldGroup>
      <FieldSet>
        {/* <FieldLegend>Range</FieldLegend>
                <FieldDescription>The range of the spell.</FieldDescription> */}
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="input-range">
              Range
              {/* <InfoPopover description="A spell's range indicates how far from the spellcaster the spell's effect can originate, and the spell's description specifies which part of the effect is limited by the range." /> */}
            </FieldLabel>
            {/* <FieldDescription>
                      A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more.
                    </FieldDescription> */}

            <Autocomplete
              id="input-range"
              items={times}
            >
              <AutocompleteInput placeholder="120 feet">
                <AutocompleteClear />
                <AutocompleteTrigger />
              </AutocompleteInput>
              <AutocompletePopup>
                <AutocompleteEmpty>No items found</AutocompleteEmpty>
                <AutocompleteList>
                  {(item: { label: string; value: string }) => (
                    <AutocompleteItem
                      key={item.value}
                      value={item.value}
                    >
                      {item.label}
                    </AutocompleteItem>
                  )}
                </AutocompleteList>
              </AutocompletePopup>
            </Autocomplete>

            {/* <FieldDescription>The time required to cast the spell.</FieldDescription> */}
          </Field>

          {/* <FieldSeparator /> */}
          {/* <Field orientation="horizontal">
                    <Checkbox id="is-ritual-spell" />
                    <FieldContent>
                      <FieldLabel htmlFor="is-ritual-spell" className="font-normal">
                        Ritual Spell
                      </FieldLabel>
                      <FieldDescription>Indicates whether the spell can be cast as a ritual.</FieldDescription>
                    </FieldContent>
                  </Field> */}
        </FieldGroup>
      </FieldSet>
    </FieldGroup>
  );
}

function DurationExample() {
  const duration = [
    { label: "Instantaneous", value: "Instantaneous" },
    {
      label: "Concentration, up to 1 minute",
      value: "Concentration, up to 1 minute",
    },
    {
      label: "Concentration, up to 10 minutes",
      value: "Concentration, up to 10 minutes",
    },
    {
      label: "Concentration, up to 1 hour",
      value: "Concentration, up to 1 hour",
    },
    {
      label: "Concentration, up to 8 hours",
      value: "Concentration, up to 8 hours",
    },
    {
      label: "Concentration, up to 24 hours",
      value: "Concentration, up to 24 hours",
    },
    { label: "1 minute", value: "1 minute" },
    { label: "10 minutes", value: "10 minutes" },
    { label: "1 hour", value: "1 hour" },
    { label: "8 hours", value: "8 hours" },
    { label: "24 hours", value: "24 hours" },
  ];

  return (
    <form>
      <FieldGroup>
        <FieldSet>
          {/* <FieldLegend>Duration</FieldLegend>
                <FieldDescription>The duration of the spell.</FieldDescription> */}
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="input-duration">
                Duration
                {/* <InfoPopover description="A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more." /> */}
              </FieldLabel>
              {/* <FieldDescription>
                      A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more.
                    </FieldDescription> */}

              <Autocomplete
                id="input-duration"
                items={duration}
              >
                <AutocompleteInput placeholder="Instantaneous">
                  <AutocompleteClear />
                  <AutocompleteTrigger />
                </AutocompleteInput>
                <AutocompletePopup>
                  <AutocompleteEmpty>No items found</AutocompleteEmpty>
                  <AutocompleteList>
                    {(item: { label: string; value: string }) => (
                      <AutocompleteItem
                        key={item.value}
                        value={item.value}
                      >
                        {item.label}
                      </AutocompleteItem>
                    )}
                  </AutocompleteList>
                </AutocompletePopup>
              </Autocomplete>

              {/* <FieldDescription>The time required to cast the spell.</FieldDescription> */}
            </Field>

            {/* <FieldSeparator /> */}
            <Field orientation="horizontal">
              <Checkbox id="is-concentration-spell" />
              <FieldContent>
                <FieldLabel
                  htmlFor="is-concentration-spell"
                  className="font-normal"
                >
                  Concentration Spell
                </FieldLabel>
                <FieldDescription>Indicates whether the spell requires concentration.</FieldDescription>
              </FieldContent>
            </Field>
          </FieldGroup>
        </FieldSet>
      </FieldGroup>
    </form>
  );
}

function InfoPopover({ description, title, className }: { title?: string; description: string; className?: string }) {
  return (
    <Popover>
      <PopoverTrigger>
        <CircleQuestionMarkIcon
          size={16}
          className={cn("text-gray-400", className)}
        />
      </PopoverTrigger>
      <PopoverContent
        side="bottom"
        sideOffset={8}
      >
        <PopoverHeader>
          {title && <PopoverTitle>{title}</PopoverTitle>}
          <PopoverDescription>{description}</PopoverDescription>
        </PopoverHeader>
      </PopoverContent>
    </Popover>
  );
}
