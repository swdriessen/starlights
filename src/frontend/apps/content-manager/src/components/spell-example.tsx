"use client";

import * as React from "react";

import { Example, ExampleWrapper } from "@/components/example";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogMedia,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Badge } from "@/components/ui/badge";
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
  PlayIcon,
  EditIcon,
  Edit2Icon,
  Edit3Icon,
  GemIcon,
  MailQuestionIcon,
  ChevronDownIcon,
  LightbulbIcon,
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
import { HoverCard, HoverCardContent, HoverCardTrigger } from "./ui/hover-card";
import TestComponent from "./test";
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
            <div className="w-full">
              <TableComponent />
            </div>
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

      <ExampleWrapper>
        <CastingTimeExample />
        <SpellLevelExample />
        <RangeExample />
        <DurationExample />
        <SpellComponentsWithToggles />

        <DescriptionComp />
        <DescriptionWithChecks />
        <DescriptionComp2 />
        <AssignClasses />
        <RelatedAbilityScoreExample />
        <SpellDetailsExample2 />
        <AssignIssue />
        <AssignTags />
      </ExampleWrapper>

      <ExampleWrapper>
        <CastingTimeExample />
        <SpellLevelExample />
        <RangeExample />
        <DurationExample />
        <SpellComponentsWithToggles />
        <CardExample />
        <DescriptionComp />
        <DescriptionWithChecks />
        <DescriptionComp2 />

        <AssignClasses />
        <RelatedAbilityScoreExample />
        {/* <SpellComponentsWithChecks /> */}
        <SpellDetailsExample2 />
        <AssignIssue />
        {/* <CastingTimeExampleOld /> */}
        <AssignTags />
        <FormExample />
        <ContributionsActivity />
        <LanguageClassification />
        <AssignLanguageOrigin />
        <Proficiency />
      </ExampleWrapper>

      <div className="container mx-auto">
        <FormExample2 />
      </div>
    </>
  );
}

function CardExample() {
  return (
    <Example
      title="Card"
      className="items-center justify-center"
    >
      <Card className="relative w-full max-w-sm overflow-hidden pt-0">
        <div className="bg-primary absolute inset-0 z-30 aspect-video opacity-50 mix-blend-color" />
        <img
          src="https://www.dndbeyond.com/attachments/12/687/spellguard-paladin.jpg"
          alt="AI-generated Kraken artwork"
          title="AI-generated Kraken artwork"
          className="relative z-20 aspect-video w-full object-cover brightness-60 grayscale"
        />
        <CardHeader>
          <CardTitle>Unearthed Arcana: Mystic Subclasses</CardTitle>
          <CardDescription>
            This playtest introduces four subclasses that explore new ways to wield, counter, steal, and bargain with magic.
          </CardDescription>
        </CardHeader>
        <CardFooter>
          <AlertDialog>
            <AlertDialogTrigger render={<Button />}>
              <FileIcon data-icon="inline-start" />
              Manage Content
            </AlertDialogTrigger>
            <AlertDialogContent size="sm">
              <AlertDialogHeader>
                <AlertDialogMedia>
                  <BluetoothIcon />
                </AlertDialogMedia>
                <AlertDialogTitle>Allow accessory to connect?</AlertDialogTitle>
                <AlertDialogDescription>Do you want to allow the USB accessory to connect to this device?</AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>Don&apos;t allow</AlertDialogCancel>
                <AlertDialogAction>Allow</AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
          <Badge
            variant="secondary"
            className="ml-auto"
          >
            Unearthed Arcana
          </Badge>
        </CardFooter>
      </Card>
    </Example>
  );
}

const frameworks = ["Next.js", "SvelteKit", "Nuxt.js", "Remix", "Astro"] as const;

const roleItems = [
  { label: "Developer", value: "developer" },
  { label: "Designer", value: "designer" },
  { label: "Manager", value: "manager" },
  { label: "Other", value: "other" },
];

function FormExample() {
  const [notifications, setNotifications] = React.useState({
    email: true,
    sms: false,
    push: true,
  });
  const [theme, setTheme] = React.useState("light");

  return (
    <Example title="Form">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>User Information</CardTitle>
          <CardDescription>Please fill in your details below</CardDescription>
          <CardAction>
            <DropdownMenu>
              <DropdownMenuTrigger
                render={
                  <Button
                    variant="ghost"
                    size="icon"
                  />
                }
              >
                <MoreVerticalIcon />
                <span className="sr-only">More options</span>
              </DropdownMenuTrigger>
              <DropdownMenuContent
                align="end"
                className="w-56"
              >
                <DropdownMenuGroup>
                  <DropdownMenuLabel>File</DropdownMenuLabel>
                  <DropdownMenuItem>
                    <FileIcon />
                    New File
                    <DropdownMenuShortcut>⌘N</DropdownMenuShortcut>
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <FolderIcon />
                    New Folder
                    <DropdownMenuShortcut>⇧⌘N</DropdownMenuShortcut>
                  </DropdownMenuItem>
                  <DropdownMenuSub>
                    <DropdownMenuSubTrigger>
                      <FolderOpenIcon />
                      Open Recent
                    </DropdownMenuSubTrigger>
                    <DropdownMenuPortal>
                      <DropdownMenuSubContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>Recent Projects</DropdownMenuLabel>
                          <DropdownMenuItem>
                            <FileCodeIcon />
                            Project Alpha
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <FileCodeIcon />
                            Project Beta
                          </DropdownMenuItem>
                          <DropdownMenuSub>
                            <DropdownMenuSubTrigger>
                              <MoreHorizontalIcon />
                              More Projects
                            </DropdownMenuSubTrigger>
                            <DropdownMenuPortal>
                              <DropdownMenuSubContent>
                                <DropdownMenuItem>
                                  <FileCodeIcon />
                                  Project Gamma
                                </DropdownMenuItem>
                                <DropdownMenuItem>
                                  <FileCodeIcon />
                                  Project Delta
                                </DropdownMenuItem>
                              </DropdownMenuSubContent>
                            </DropdownMenuPortal>
                          </DropdownMenuSub>
                        </DropdownMenuGroup>
                        <DropdownMenuSeparator />
                        <DropdownMenuGroup>
                          <DropdownMenuItem>
                            <FolderSearchIcon />
                            Browse...
                          </DropdownMenuItem>
                        </DropdownMenuGroup>
                      </DropdownMenuSubContent>
                    </DropdownMenuPortal>
                  </DropdownMenuSub>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem>
                    <SaveIcon />
                    Save
                    <DropdownMenuShortcut>⌘S</DropdownMenuShortcut>
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <DownloadIcon />
                    Export
                    <DropdownMenuShortcut>⇧⌘E</DropdownMenuShortcut>
                  </DropdownMenuItem>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuLabel>View</DropdownMenuLabel>
                  <DropdownMenuCheckboxItem
                    checked={notifications.email}
                    onCheckedChange={(checked) =>
                      setNotifications({
                        ...notifications,
                        email: checked === true,
                      })
                    }
                  >
                    <EyeIcon />
                    Show Sidebar
                  </DropdownMenuCheckboxItem>
                  <DropdownMenuCheckboxItem
                    checked={notifications.sms}
                    onCheckedChange={(checked) =>
                      setNotifications({
                        ...notifications,
                        sms: checked === true,
                      })
                    }
                  >
                    <LayoutIcon />
                    Show Status Bar
                  </DropdownMenuCheckboxItem>
                  <DropdownMenuSub>
                    <DropdownMenuSubTrigger>
                      <PaletteIcon />
                      Theme
                    </DropdownMenuSubTrigger>
                    <DropdownMenuPortal>
                      <DropdownMenuSubContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>Appearance</DropdownMenuLabel>
                          <DropdownMenuRadioGroup
                            value={theme}
                            onValueChange={setTheme}
                          >
                            <DropdownMenuRadioItem value="light">
                              <SunIcon />
                              Light
                            </DropdownMenuRadioItem>
                            <DropdownMenuRadioItem value="dark">
                              <MoonIcon />
                              Dark
                            </DropdownMenuRadioItem>
                            <DropdownMenuRadioItem value="system">
                              <MonitorIcon />
                              System
                            </DropdownMenuRadioItem>
                          </DropdownMenuRadioGroup>
                        </DropdownMenuGroup>
                      </DropdownMenuSubContent>
                    </DropdownMenuPortal>
                  </DropdownMenuSub>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuLabel>Account</DropdownMenuLabel>
                  <DropdownMenuItem>
                    <UserIcon />
                    Profile
                    <DropdownMenuShortcut>⇧⌘P</DropdownMenuShortcut>
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <CreditCardIcon />
                    Billing
                  </DropdownMenuItem>
                  <DropdownMenuSub>
                    <DropdownMenuSubTrigger>
                      <SettingsIcon />
                      Settings
                    </DropdownMenuSubTrigger>
                    <DropdownMenuPortal>
                      <DropdownMenuSubContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>Preferences</DropdownMenuLabel>
                          <DropdownMenuItem>
                            <KeyboardIcon />
                            Keyboard Shortcuts
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <LanguagesIcon />
                            Language
                          </DropdownMenuItem>
                          <DropdownMenuSub>
                            <DropdownMenuSubTrigger>
                              <BellIcon />
                              Notifications
                            </DropdownMenuSubTrigger>
                            <DropdownMenuPortal>
                              <DropdownMenuSubContent>
                                <DropdownMenuGroup>
                                  <DropdownMenuLabel>Notification Types</DropdownMenuLabel>
                                  <DropdownMenuCheckboxItem
                                    checked={notifications.push}
                                    onCheckedChange={(checked) =>
                                      setNotifications({
                                        ...notifications,
                                        push: checked === true,
                                      })
                                    }
                                  >
                                    <BellIcon />
                                    Push Notifications
                                  </DropdownMenuCheckboxItem>
                                  <DropdownMenuCheckboxItem
                                    checked={notifications.email}
                                    onCheckedChange={(checked) =>
                                      setNotifications({
                                        ...notifications,
                                        email: checked === true,
                                      })
                                    }
                                  >
                                    <MailIcon />
                                    Email Notifications
                                  </DropdownMenuCheckboxItem>
                                </DropdownMenuGroup>
                              </DropdownMenuSubContent>
                            </DropdownMenuPortal>
                          </DropdownMenuSub>
                        </DropdownMenuGroup>
                        <DropdownMenuSeparator />
                        <DropdownMenuGroup>
                          <DropdownMenuItem>
                            <ShieldIcon />
                            Privacy & Security
                          </DropdownMenuItem>
                        </DropdownMenuGroup>
                      </DropdownMenuSubContent>
                    </DropdownMenuPortal>
                  </DropdownMenuSub>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuItem>
                    <HelpCircleIcon />
                    Help & Support
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <FileTextIcon />
                    Documentation
                  </DropdownMenuItem>
                </DropdownMenuGroup>
                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuItem variant="destructive">
                    <LogOutIcon />
                    Sign Out
                    <DropdownMenuShortcut>⇧⌘Q</DropdownMenuShortcut>
                  </DropdownMenuItem>
                </DropdownMenuGroup>
              </DropdownMenuContent>
            </DropdownMenu>
          </CardAction>
        </CardHeader>
        <CardContent>
          <form>
            <FieldGroup>
              <div className="grid grid-cols-2 gap-4">
                <Field>
                  <FieldLabel htmlFor="small-form-name">Name</FieldLabel>
                  <Input
                    id="small-form-name"
                    placeholder="Enter your name"
                    required
                  />
                </Field>
                <Field>
                  <FieldLabel htmlFor="small-form-role">Role</FieldLabel>
                  <Select
                    items={roleItems}
                    defaultValue={null}
                  >
                    <SelectTrigger id="small-form-role">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectGroup>
                        {roleItems.map((item) => (
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
              </div>
              <Field>
                <FieldLabel htmlFor="small-form-framework">Framework</FieldLabel>
                <Combobox items={frameworks}>
                  <ComboboxInput
                    id="small-form-framework"
                    placeholder="Select a framework"
                    required
                  />
                  <ComboboxContent>
                    <ComboboxEmpty>No frameworks found.</ComboboxEmpty>
                    <ComboboxList>
                      {(item) => (
                        <ComboboxItem
                          key={item}
                          value={item}
                        >
                          {item}
                        </ComboboxItem>
                      )}
                    </ComboboxList>
                  </ComboboxContent>
                </Combobox>
              </Field>
              <Field>
                <FieldLabel htmlFor="small-form-comments">Comments</FieldLabel>
                <Textarea
                  id="small-form-comments"
                  placeholder="Add any additional comments"
                />
              </Field>
              <Field orientation="horizontal">
                <Button type="submit">Submit</Button>
                <Button
                  variant="outline"
                  type="button"
                >
                  Cancel
                </Button>
              </Field>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}
const users = ["Abjuration", "Evocation", "Ritual", "Concentration", "Somatic", "Verbal", "Material"];

function AssignIssue() {
  const anchor = useComboboxAnchor();

  return (
    <Example
      title="User Select"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-sm"
        size="sm"
      >
        <CardHeader className="border-b">
          <CardTitle className="text-sm">Assign Label</CardTitle>
          <CardDescription className="text-sm">Select labels to assign to this element.</CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    variant="outline"
                    size="icon-xs"
                  />
                }
              >
                <PlusIcon />
              </TooltipTrigger>
              <TooltipContent>Add user</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader>
        <CardContent>
          <Combobox
            multiple
            autoHighlight
            items={users}
            defaultValue={[users[0]]}
          >
            <ComboboxChips ref={anchor}>
              <ComboboxValue>
                {(values) => (
                  <React.Fragment>
                    {values.map((username: string) => (
                      <ComboboxChip key={username}>
                        <Avatar className="size-4">
                          <AvatarImage
                            src={`https://github.com/${username}.png`}
                            alt={username}
                          />
                          <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                        </Avatar>
                        {username}
                      </ComboboxChip>
                    ))}
                    <ComboboxChipsInput placeholder={values.length > 0 ? undefined : "Select a item..."} />
                  </React.Fragment>
                )}
              </ComboboxValue>
            </ComboboxChips>
            <ComboboxContent anchor={anchor}>
              <ComboboxEmpty>No users found.</ComboboxEmpty>
              <ComboboxList>
                {(username) => (
                  <ComboboxItem
                    key={username}
                    value={username}
                  >
                    <Avatar className="size-5">
                      <AvatarImage
                        src={`https://github.com/${username}.png`}
                        alt={username}
                      />
                      <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                    </Avatar>
                    {username}
                  </ComboboxItem>
                )}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
        </CardContent>
      </Card>
    </Example>
  );
}

const tags = ["Attunement", "Concentration", "Ritual", "Verbal", "Somatic", "Material", "Art Object", "Currency"];

function AssignTags() {
  const anchor = useComboboxAnchor();

  return (
    <Example
      title="Tag Select"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-sm"
        size="default"
      >
        <CardHeader className="border-b">
          <CardTitle className="text-sm">Assign Tags</CardTitle>
          <CardDescription className="text-sm">Select tags to assign to this element.</CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    variant="outline"
                    size="icon-xs"
                  />
                }
              >
                <PlusIcon />
              </TooltipTrigger>
              <TooltipContent>Add tag</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader>
        <CardContent>
          <Combobox
            multiple
            autoHighlight
            items={tags}
            defaultValue={[tags[0]]}
          >
            <ComboboxChips ref={anchor}>
              <ComboboxValue>
                {(values) => (
                  <React.Fragment>
                    {values.map((username: string) => (
                      <ComboboxChip key={username}>
                        {/* <Avatar className="size-4">
                          <AvatarImage src={`https://github.com/${username}.png`} alt={username} />
                          <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                        </Avatar> */}
                        {username}
                      </ComboboxChip>
                    ))}
                    <ComboboxChipsInput placeholder={values.length > 0 ? undefined : "Select a tag..."} />
                  </React.Fragment>
                )}
              </ComboboxValue>
            </ComboboxChips>
            <ComboboxContent anchor={anchor}>
              <ComboboxEmpty>No tags found.</ComboboxEmpty>
              <ComboboxList>
                {(username) => (
                  <ComboboxItem
                    key={username}
                    value={username}
                  >
                    {/* <Avatar className="size-5">
                      <AvatarImage src={`https://github.com/${username}.png`} alt={username} />
                      <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                    </Avatar> */}
                    {username}
                  </ComboboxItem>
                )}
              </ComboboxList>
            </ComboboxContent>
          </Combobox>
        </CardContent>
      </Card>
    </Example>
  );
}
function FormExample2() {
  const monthItems = [
    { label: "MM", value: null },
    { label: "01", value: "01" },
    { label: "02", value: "02" },
    { label: "03", value: "03" },
    { label: "04", value: "04" },
    { label: "05", value: "05" },
    { label: "06", value: "06" },
    { label: "07", value: "07" },
    { label: "08", value: "08" },
    { label: "09", value: "09" },
    { label: "10", value: "10" },
    { label: "11", value: "11" },
    { label: "12", value: "12" },
  ];

  const yearItems = [
    { label: "YYYY", value: null },
    { label: "2024", value: "2024" },
    { label: "2025", value: "2025" },
    { label: "2026", value: "2026" },
    { label: "2027", value: "2027" },
    { label: "2028", value: "2028" },
    { label: "2029", value: "2029" },
  ];

  return (
    <Example title="Spell Details Form">
      <Card className="w-full ">
        <CardHeader>
          <CardTitle>Spell Details</CardTitle>
          <CardDescription>All transactions are secure and encrypted</CardDescription>
        </CardHeader>
        <CardContent>
          <form>
            <FieldGroup>
              <FieldSet>
                <FieldGroup>
                  <Field>
                    <FieldLabel htmlFor="checkout-7j9-card-name-43j">Name</FieldLabel>
                    <Input
                      id="checkout-7j9-card-name-43j"
                      placeholder="Fireball"
                      required
                    />
                  </Field>
                  <div className="grid grid-cols-3 gap-4">
                    <Field className="col-span-2">
                      <FieldLabel htmlFor="checkout-7j9-card-number-uw1">Card Number</FieldLabel>
                      <Input
                        id="checkout-7j9-card-number-uw1"
                        placeholder="1234 5678 9012 3456"
                        required
                      />
                      <FieldDescription>Enter your 16-digit number.</FieldDescription>
                    </Field>
                    <Field className="col-span-1">
                      <FieldLabel htmlFor="checkout-7j9-cvv">CVV</FieldLabel>
                      <Input
                        id="checkout-7j9-cvv"
                        placeholder="123"
                        required
                      />
                    </Field>
                  </div>
                  <div className="grid grid-cols-2 gap-4">
                    <Field>
                      <FieldLabel htmlFor="checkout-7j9-exp-month-ts6">Month</FieldLabel>
                      <Select
                        items={monthItems}
                        defaultValue={null}
                      >
                        <SelectTrigger id="checkout-7j9-exp-month-ts6">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            {monthItems.map((item) => (
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
                    <Field>
                      <FieldLabel htmlFor="checkout-7j9-exp-year-f59">Year</FieldLabel>
                      <Select
                        items={yearItems}
                        defaultValue={null}
                      >
                        <SelectTrigger id="checkout-7j9-exp-year-f59">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            {yearItems.map((item) => (
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
                  </div>
                </FieldGroup>
              </FieldSet>
              <FieldSeparator />
              <FieldSet>
                <FieldLegend>Billing Address</FieldLegend>
                <FieldDescription>The billing address associated with your payment.</FieldDescription>
                <FieldGroup>
                  <Field orientation="horizontal">
                    <Checkbox
                      id="checkout-7j9-same-as-shipping-wgm"
                      defaultChecked
                    />
                    <FieldLabel
                      htmlFor="checkout-7j9-same-as-shipping-wgm"
                      className="font-normal"
                    >
                      Same as shipping address
                    </FieldLabel>
                  </Field>
                </FieldGroup>
              </FieldSet>
              <FieldSeparator />
              <FieldSet>
                <FieldGroup>
                  <Field>
                    <FieldLabel htmlFor="checkout-7j9-optional-comments">Comments</FieldLabel>
                    <Textarea
                      id="checkout-7j9-optional-comments"
                      placeholder="Add any additional comments"
                    />
                  </Field>
                </FieldGroup>
              </FieldSet>
              <Field orientation="horizontal">
                <Button type="submit">Submit</Button>
                <Button
                  variant="outline"
                  type="button"
                >
                  Cancel
                </Button>
              </Field>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}

function SpellComponentsWithChecks() {
  return (
    <Example
      title="Spell Components + Checkboxes"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-md "
        size="default"
      >
        {/* <CardHeader className="border-b">
          <CardTitle>Spell Components</CardTitle>
          <CardDescription>A spell's components are physical requirements the spellcaster must meet to cast the spell.</CardDescription>
        </CardHeader> */}

        <CardContent>
          <form id="spell-components-form">
            <FieldGroup>
              <FieldSet>
                <FieldGroup>
                  <Field>
                    <FieldContent>
                      <FieldLabel htmlFor="terms-1">Spell Components</FieldLabel>
                      <FieldDescription>
                        A spell's components are physical requirements the spellcaster must meet to cast the spell.
                      </FieldDescription>
                    </FieldContent>
                    {/* 
                    <div className="flex items-center gap-2 mt-2">
                      <Tooltip>
                        <TooltipTrigger render={<Toggle variant="outline" size="sm" />}>V</TooltipTrigger>
                        <TooltipContent sideOffset={10}>Verbal Component</TooltipContent>
                      </Tooltip>
                      <Tooltip>
                        <TooltipTrigger render={<Toggle variant="outline" size="sm" />}>S</TooltipTrigger>
                        <TooltipContent sideOffset={10}>Somatic Component</TooltipContent>
                      </Tooltip>
                      <Tooltip>
                        <TooltipTrigger render={<Toggle variant="outline" size="sm" />}>M</TooltipTrigger>
                        <TooltipContent sideOffset={10}>Material Component</TooltipContent>
                      </Tooltip>
                    </div> */}
                  </Field>

                  <Field orientation="horizontal">
                    <Checkbox id="terms-1"></Checkbox>
                    <FieldContent>
                      <FieldLabel htmlFor="terms-1">Verbal</FieldLabel>
                      {/* <FieldDescription>Chanting of esoteric words that sound like nonsense to the uninitiated.</FieldDescription> */}
                    </FieldContent>
                    <Checkbox id="terms-2" />
                    <FieldContent>
                      <FieldLabel htmlFor="terms-2">Somatic</FieldLabel>
                      {/* <FieldDescription>A forceful gesticulation or an intricate set of gestures.</FieldDescription> */}
                    </FieldContent>
                    <Checkbox id="terms-3" />
                    <FieldContent>
                      <FieldLabel htmlFor="terms-3">Material</FieldLabel>
                      {/* <FieldDescription>A particular material used in a spell's casting.</FieldDescription> */}
                    </FieldContent>
                  </Field>

                  <Field className="">
                    <FieldLabel htmlFor="material-component-description">Material Component Description</FieldLabel>
                    <Textarea
                      id="material-component-description"
                      placeholder="a bit of flour"
                    />
                  </Field>
                </FieldGroup>
              </FieldSet>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}

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

function SpellDescription({ name, level, school }: { name: string; level: number; school: string }) {
  return (
    <>
      <div className="prose prose-neutral dark:prose-invert max-w-none text-sm">
        <h2 className="uppercase">{name}</h2>
        <p className="-mt-5 ">
          {level === 0 && <span className="italic">{school} Cantrip </span>}
          {level > 0 && (
            <span className="italic">
              Level {level} {school}{" "}
            </span>
          )}
          {/* <span className="italic">(Sorcerer, Wizard)</span> */}
        </p>
        <div className="mb-3">
          <ul className="not-prose">
            <li className="not-prose">
              <span className="font-bold">Casting Time:</span> 1 Action
            </li>
            <li className="not-prose">
              <span className="font-bold">Range:</span> 60 feet
            </li>
            <li className="not-prose">
              <span className="font-bold">Components:</span> V, S, M (a bit of flour)
            </li>
            <li className="not-prose">
              <span className="font-bold">Duration:</span> Instantaneous
            </li>
          </ul>
        </div>
      </div>
    </>
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

function DescriptionCompPreview() {
  const [description, setDescription] = React.useState("test markdown");
  return (
    <>
      {/* <CardHeader className="border-b">
          <CardTitle>Spell Components</CardTitle>
          <CardDescription>A spell's components are physical requirements the spellcaster must meet to cast the spell.</CardDescription>
        </CardHeader> */}

      <Card
        className="w-full max-w-md "
        size="default"
      >
        <CardContent>
          <SpellDescription
            name="Acid Splash"
            level={0}
            school="Evocation"
          />

          <div className="prose prose-neutral dark:prose-invert max-w-none text-sm">
            <Markdown
              remarkPlugins={[remarkGfm]}
              components={{
                // Map `h1` (`# heading`) to use `h2`s.
                h1: "h2",
                // Rewrite `em`s (`*like so*`) to `i` with a red foreground color.
                em(props) {
                  const { node, ...rest } = props;
                  return (
                    <i
                      style={{ color: "red" }}
                      {...rest}
                    />
                  );
                },
              }}
            >
              {description}
            </Markdown>
            <a
              href="https://www.dndbeyond.com/spells/acid-splash"
              className="text-sm"
            >
              Player's Handbook
            </a>
          </div>
        </CardContent>
      </Card>
    </>
  );
}
function DescriptionWithChecks() {
  const [description, setDescription] = React.useState("");
  return (
    <Example
      title="Description Fields + Menu"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-md "
        size="default"
      >
        {/* <CardHeader className="border-b">
          <CardTitle>Spell Components</CardTitle>
          <CardDescription>A spell's components are physical requirements the spellcaster must meet to cast the spell.</CardDescription>
        </CardHeader> */}
        <CardHeader className="border-b">
          <CardTitle>Description</CardTitle>
          <CardDescription>
            Your description can only contain plain text.
            {/* <InfoPopover description="Descriptions can contain markdown syntax. In a later version, this will be a rich text editor." /> */}
          </CardDescription>
          <CardAction>
            <DropdownMenu>
              <DropdownMenuTrigger
                render={
                  <Button
                    variant="ghost"
                    size="icon"
                  />
                }
              >
                <MoreVerticalIcon />
                <span className="sr-only">More options</span>
              </DropdownMenuTrigger>
              <DropdownMenuContent
                align="end"
                className="w-56"
              >
                <DropdownMenuGroup>
                  <DropdownMenuLabel>Description Snippets</DropdownMenuLabel>
                  {/* <DropdownMenuItem>
                    <IndentIcon />
                    Insert Demo Snippet
                  </DropdownMenuItem> */}
                  <DropdownMenuSub>
                    <DropdownMenuSubTrigger>
                      <IndentIcon />
                      Insert Snippet
                    </DropdownMenuSubTrigger>
                    <DropdownMenuPortal>
                      <DropdownMenuSubContent>
                        <DropdownMenuGroup>
                          <DropdownMenuLabel>Snippets</DropdownMenuLabel>
                          <DropdownMenuItem
                            onClick={() => {
                              // append text
                              const text = "Using a Higher-Level Spell Slot.";
                              setDescription(description + text);
                            }}
                          >
                            <IndentIcon />
                            Higher-Level Spell Slot
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <IndentIcon />
                            Cantrip Upgrade
                          </DropdownMenuItem>
                          {/* <DropdownMenuSub>
                            <DropdownMenuSubTrigger>
                              <BellIcon />
                              Notifications
                            </DropdownMenuSubTrigger>
                            <DropdownMenuPortal>
                              <DropdownMenuSubContent>
                                <DropdownMenuGroup>
                                  <DropdownMenuLabel>
                                    Notification Types
                                  </DropdownMenuLabel>
                                </DropdownMenuGroup>
                              </DropdownMenuSubContent>
                            </DropdownMenuPortal>
                          </DropdownMenuSub> */}
                        </DropdownMenuGroup>
                        <DropdownMenuSeparator />
                        <DropdownMenuGroup>
                          <DropdownMenuItem
                            onClick={() => {
                              const demo =
                                "You create an acidic bubble at a point within range, where it explodes in a 5-foot-radius Sphere. Each creature in that Sphere must succeed on a Dexterity saving throw or take 1d6 Acid damage.\r\n___Cantrip Upgrade.___ The damage increases by 1d6 when you reach levels 5 (2d6), 11 (3d6), and 17 (4d6).";

                              setDescription(demo);
                            }}
                          >
                            <IndentIcon />
                            Demo Snippet
                          </DropdownMenuItem>
                        </DropdownMenuGroup>
                      </DropdownMenuSubContent>
                    </DropdownMenuPortal>
                  </DropdownMenuSub>
                </DropdownMenuGroup>

                <DropdownMenuSeparator />
                <DropdownMenuGroup>
                  <DropdownMenuItem>
                    <HelpCircleIcon />
                    Markdown Guide
                  </DropdownMenuItem>
                </DropdownMenuGroup>
              </DropdownMenuContent>
            </DropdownMenu>
          </CardAction>
        </CardHeader>

        {/* <CardHeader className="border-b">
          <CardTitle className="text-sm">Description</CardTitle>
          <CardDescription className="text-sm">
            Select labels to assign to this element.
          </CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger
                render={<Button variant="ghost" size="icon-sm" />}
              >
                <MoreVerticalIcon />
              </TooltipTrigger>
              <TooltipContent>Add user</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader> */}

        <CardContent>
          {/* tabs with preview */}
          <form id="spell-components-form">
            <FieldGroup>
              <FieldSet className="">
                <FieldGroup className="">
                  {/* <Field className="">
                    <FieldContent>
                      <FieldLabel htmlFor="terms-1">Description</FieldLabel>
                      <FieldDescription>The description of the spell should only contain</FieldDescription>
                    </FieldContent>
                  </Field> */}

                  <Field className="">
                    {/* <FieldLabel htmlFor="material-component-description">
                      Description
                    </FieldLabel> */}
                    <Textarea
                      id="material-component-description"
                      className="min-h-60"
                      placeholder="You create an acidic bubble at a point within range, where it explodes in a 5-foot-radius Sphere. Each creature in that Sphere must succeed on a Dexterity saving throw or take 1d6 Acid damage.
___Cantrip Upgrade.___ The damage increases by 1d6 when you reach levels 5 (2d6), 11 (3d6), and 17 (4d6)."
                    />
                  </Field>

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
        </CardContent>
      </Card>
    </Example>
  );
}

function DescriptionComp2() {
  return (
    <Example
      title="Description Fields + Accordion"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-md "
        size="default"
      >
        <CardContent>
          <form id="spell-components-form">
            <FieldGroup>
              <FieldSet>
                <FieldGroup>
                  {/* <Field className="">
                    <FieldContent>
                      <FieldLabel htmlFor="terms-1">Description
                        
                      </FieldLabel>
                      <FieldDescription>
                        The description of the spell should only contain
                      </FieldDescription>
                    </FieldContent>
                  </Field> */}

                  <Field className="">
                    <FieldLabel htmlFor="material-component-description">
                      Description
                      <InfoPopover description="The description of the spell can rich text with markdown formatting." />
                    </FieldLabel>
                    <Textarea
                      id="material-component-description"
                      className="min-h-50"
                      placeholder="You create an acidic bubble at a point within range, where it explodes in a 5-foot-radius Sphere. Each creature in that Sphere must succeed on a Dexterity saving throw or take 1d6 Acid damage."
                    />
                  </Field>
                  {/* 
                  <Field className="">
                    <FieldLabel htmlFor="material-component-description">Using a Higher-Level Spell Slot</FieldLabel>
                    <Textarea id="material-component-description" placeholder="Each target's Hit Points increase by 5 for each spell slot level above 2." />
                  </Field>

                  <Field className="">
                    <FieldLabel htmlFor="material-component-description">Cantrip Upgrade</FieldLabel>
                  </Field> */}
                </FieldGroup>

                <FieldGroup>
                  <Accordion className="">
                    {/* <AccordionItem>
                      <AccordionTrigger>Description</AccordionTrigger>
                      <AccordionContent>
                        <Textarea
                          id="material-component-description"
                          className="min-h-50"
                          placeholder="You create an acidic bubble at a point within range, where it explodes in a 5-foot-radius Sphere. Each creature in that Sphere must succeed on a Dexterity saving throw or take 1d6 Acid damage."
                        />
                      </AccordionContent>
                    </AccordionItem> */}
                    <AccordionItem>
                      <AccordionTrigger>Using a Higher-Level Spell Slot</AccordionTrigger>
                      <AccordionContent>
                        <Textarea
                          id="material-component-description1"
                          className=""
                          placeholder="Each target's Hit Points increase by 5 for each spell slot level above 2."
                        />
                      </AccordionContent>
                    </AccordionItem>
                    <AccordionItem>
                      <AccordionTrigger>Cantrip Update</AccordionTrigger>
                      <AccordionContent>
                        <Textarea
                          id="material-component-description2"
                          placeholder="The damage increases by 1d6 when you reach levels 5 (2d6),11 (3d6), and 17 (4d6)."
                        />
                      </AccordionContent>
                    </AccordionItem>
                  </Accordion>
                </FieldGroup>
              </FieldSet>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}

function LanguageClassification() {
  return (
    <Example
      title="User Select"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-sm"
        size="sm"
      >
        <CardHeader className="border-b">
          <CardTitle className="text-sm">Language Classification</CardTitle>
          <CardDescription className="text-sm">Select from standard or rare.</CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    variant="outline"
                    size="icon-xs"
                  />
                }
              >
                <PlusIcon />
              </TooltipTrigger>
              <TooltipContent>Add user</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader>
        <CardContent>
          <FieldSet>
            {/* <FieldLegend>Compute Environment</FieldLegend>
            <FieldDescription>Select the compute environment for your cluster.</FieldDescription> */}
            <RadioGroup defaultValue="standard">
              <FieldLabel htmlFor="standard-r2h">
                <Field orientation="horizontal">
                  <FieldContent>
                    <FieldTitle>Standard</FieldTitle>
                    <FieldDescription>A language that is widespread.</FieldDescription>
                  </FieldContent>
                  <RadioGroupItem
                    value="standard"
                    id="standard-r2h"
                    aria-label="standard"
                  />
                </Field>
              </FieldLabel>
              <FieldLabel htmlFor="rare-z4k">
                <Field orientation="horizontal">
                  <FieldContent>
                    <FieldTitle>Rare</FieldTitle>
                    <FieldDescription>A language that is either secret or derived from other planes of existence.</FieldDescription>
                  </FieldContent>
                  <RadioGroupItem
                    value="rare"
                    id="rare-z4k"
                    aria-label="Rare"
                  />
                </Field>
              </FieldLabel>
            </RadioGroup>
          </FieldSet>
        </CardContent>
      </Card>
    </Example>
  );
}

function AssignLanguageOrigin() {
  const anchor = useComboboxAnchor();

  return (
    <Example
      title="Language Origin"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-sm"
        size="sm"
      >
        <CardHeader className="border-b">
          <CardTitle className="text-sm">Language Origin</CardTitle>
          <CardDescription className="text-sm">The origin or source of the language.</CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger
                render={
                  <Button
                    variant="outline"
                    size="icon-xs"
                  />
                }
              >
                {/* <PlusIcon /> */}
                <MailQuestionIcon />
              </TooltipTrigger>
              <TooltipContent>Premium Language</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader>
        <CardContent>
          <Field>
            {/* <FieldLabel htmlFor="checkout-7j9-optional-comments">Language Origin</FieldLabel> */}
            <Textarea
              id="checkout-7j9-optional-comments"
              placeholder="Devils of the Nine Hells"
            />
          </Field>
        </CardContent>
      </Card>
    </Example>
  );
}

function ContributionsActivity() {
  return (
    <Card
      className="mx-auto w-full max-w-md"
      size="default"
    >
      <CardHeader>
        <CardTitle>Properties</CardTitle>
        <CardDescription>Manage your spell properties.</CardDescription>
      </CardHeader>
      <CardContent>
        <form id="contributions-activity">
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
                </Field>
                <Field orientation="horizontal">
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
                      Your contribution graph, achievements, and activity overview will show your private contributions without revealing
                      any repository or organization information. <a href="#read-more">Read more</a>.
                    </FieldDescription>
                  </FieldContent>
                </Field> */}

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
          </FieldGroup>
        </form>
      </CardContent>
      {/* <CardFooter>
        <Button form="contributions-activity">Save Changes</Button>
      </CardFooter> */}
    </Card>
  );
}

function Proficiency() {
  return (
    <Example
      title="Proficiency"
      className="justify-center"
    >
      <Card
        className="mx-auto w-full max-w-md"
        size="default"
      >
        <CardHeader className="border-b">
          <CardTitle>Proficiency Generation</CardTitle>
          <CardDescription>Manage your proficiency settings.</CardDescription>
        </CardHeader>
        <CardContent>
          <form id="contributions-activity">
            <FieldGroup>
              <FieldSet>
                <FieldLegend className="sr-only">Contributions & activity</FieldLegend>
                <FieldGroup className="gap-3">
                  {/* <Field orientation="horizontal">
                    <Checkbox id="private-profile" />
                    <FieldContent>
                      <FieldLabel htmlFor="private-profile" className="font-normal">
                        Make profile private and hide activity
                      </FieldLabel>
                      <FieldDescription>
                        Enabling this will hide your contributions and activity from your GitHub profile and from social features like followers, stars, feeds,
                        leaderboards and releases.
                      </FieldDescription>
                    </FieldContent>
                  </Field> */}
                  <Field orientation="horizontal">
                    <Checkbox
                      id="private-contributions"
                      defaultChecked
                    />
                    <FieldContent>
                      <FieldLabel
                        htmlFor="private-contributions"
                        className="font-normal"
                      >
                        Include Proficiency
                      </FieldLabel>
                      <FieldDescription>Automatically create a proficiency element when creating this skill.</FieldDescription>
                    </FieldContent>
                  </Field>
                </FieldGroup>
              </FieldSet>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}

function SpellDetailsExample2() {
  const monthItems = [
    { label: "Cantrip", value: "0" },
    { label: "1", value: "1" },
    { label: "2", value: "2" },
    { label: "3", value: "3" },
    { label: "4", value: "4" },
    { label: "5", value: "5" },
    { label: "6", value: "6" },
    { label: "7", value: "7" },
    { label: "8", value: "8" },
    { label: "9", value: "9" },
  ];

  const yearItems = [
    // { label: "x", value: null },
    { label: "Evocation", value: "Evocation" },
    { label: "Abjuration", value: "Abjuration" },
    { label: "Conjuration", value: "Conjuration" },
    { label: "Divination", value: "Divination" },
    { label: "Enchantment", value: "Enchantment" },
    { label: "Illusion", value: "Illusion" },
    { label: "Necromancy", value: "Necromancy" },
    { label: "Transmutation", value: "Transmutation" },
  ];

  const times = [
    // { label: "x", value: null },
    { label: "Action", value: "Action" },
    { label: "Bonus Action", value: "Bonus Action" },
    { label: "Reaction", value: "Reaction" },
    { label: "1 Minute", value: "1 Minute" },
    { label: "10 Minutes", value: "10 Minutes" },
    { label: "1 Hour", value: "1 Hour" },
    { label: "8 Hours", value: "8 Hours" },
    { label: "12 Hours", value: "12 Hours" },
    { label: "24 Hours", value: "24 Hours" },
    { label: "Custom", value: "Custom" },
  ];
  const ranges = [
    // { label: "x", value: null },
    { label: "Ranged", value: "Ranged" },
    { label: "Self", value: "Self" },
    { label: "Touch", value: "Touch" },
    { label: "30 feet", value: "30 feet" },
    { label: "60 feet", value: "60 feet" },
    { label: "90 feet", value: "90 feet" },
    { label: "120 feet", value: "120 feet" },
    { label: "150 feet", value: "150 feet" },
    { label: "300 feet", value: "300 feet" },
    { label: "500 feet", value: "500 feet" },
    { label: "1 Mile", value: "1 Mile" },
    { label: "Custom", value: "Custom" },
  ];

  return (
    <Example title="Spell Details Form">
      <Card className="w-full ">
        {/* <CardHeader className="border-b">
          <CardTitle>Spell Details</CardTitle>
          <CardDescription>A series of entries that provide the details needed to cast the spell.</CardDescription>
        </CardHeader> */}
        <CardContent>
          <form>
            <FieldGroup>
              <FieldSet>
                <FieldGroup>
                  <div className="grid grid-cols-2 gap-4">
                    <Field>
                      <FieldLabel htmlFor="checkout-7j9-exp-month-ts6">Spell Level</FieldLabel>
                      <Select
                        items={monthItems}
                        defaultValue={"3"}
                      >
                        <SelectTrigger id="checkout-7j9-exp-month-ts6">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            {monthItems.map((item) => (
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
                    <Field>
                      <FieldLabel htmlFor="checkout-7j9-exp-year-f59">School of Magic</FieldLabel>
                      <Select
                        items={yearItems}
                        defaultValue={"Evocation"}
                      >
                        <SelectTrigger id="checkout-7j9-exp-year-f59">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            {yearItems.map((item) => (
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
                  </div>
                </FieldGroup>
              </FieldSet>
              <FieldSeparator />
              <FieldSet>
                {/* <FieldLegend>Casting Time</FieldLegend>
                <FieldDescription>The billing address associated with your payment.</FieldDescription> */}
                <FieldGroup>
                  {/* <Field orientation="horizontal">
                    <Checkbox id="checkout-7j9-same-as-shipping-wgm" defaultChecked />
                    <FieldLabel htmlFor="checkout-7j9-same-as-shipping-wgm" className="font-normal">
                      Ritual Spell
                    </FieldLabel>
                  </Field> */}

                  <Field>
                    <FieldLabel htmlFor="checkout-7j9-exp-year-f59">Casting Time</FieldLabel>
                    <Select
                      items={times}
                      defaultValue={"Action"}
                    >
                      <SelectTrigger id="checkout-7j9-exp-year-f59">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectGroup>
                          {times.map((item) => (
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

                  <Field orientation="horizontal">
                    <Checkbox id="private-contributions" />
                    <FieldContent>
                      <FieldLabel
                        htmlFor="private-contributions"
                        className="font-normal"
                      >
                        Ritual Spell
                      </FieldLabel>
                      <FieldDescription>Indicates whether the spell can be cast as a ritual.</FieldDescription>
                    </FieldContent>
                  </Field>
                </FieldGroup>
              </FieldSet>
              <FieldSeparator />
              <FieldSet>
                {/* <FieldLegend>Casting Time</FieldLegend>
                <FieldDescription>The billing address associated with your payment.</FieldDescription> */}
                <FieldGroup>
                  {/* <Field orientation="horizontal">
                    <Checkbox id="checkout-7j9-same-as-shipping-wgm" defaultChecked />
                    <FieldLabel htmlFor="checkout-7j9-same-as-shipping-wgm" className="font-normal">
                      Ritual Spell
                    </FieldLabel>
                  </Field> */}

                  <Field>
                    <FieldLabel htmlFor="checkout-7j9-exp-year-f59">Range</FieldLabel>
                    <Select
                      items={ranges}
                      defaultValue={"Ranged"}
                    >
                      <SelectTrigger id="checkout-7j9-exp-year-f59">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectGroup>
                          {ranges.map((item) => (
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
                </FieldGroup>
              </FieldSet>
              <FieldSeparator />
              <FieldSet>
                {/* <FieldLegend>Casting Time</FieldLegend>
                <FieldDescription>The billing address associated with your payment.</FieldDescription> */}
                <FieldGroup>
                  {/* <Field orientation="horizontal">
                    <Checkbox id="checkout-7j9-same-as-shipping-wgm" defaultChecked />
                    <FieldLabel htmlFor="checkout-7j9-same-as-shipping-wgm" className="font-normal">
                      Ritual Spell
                    </FieldLabel>
                  </Field> */}

                  <Field>
                    <FieldLabel htmlFor="checkout-7j93-exp-year-f59">Duration</FieldLabel>
                    <Select
                      items={times}
                      defaultValue={"Action"}
                    >
                      <SelectTrigger id="checkout-7j93-exp-year-f59">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectGroup>
                          {times.map((item) => (
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

                  <Field orientation="horizontal">
                    <Checkbox id="private-contributions" />
                    <FieldContent>
                      <FieldLabel
                        htmlFor="private-contributions"
                        className="font-normal"
                      >
                        Concentration Spell
                      </FieldLabel>
                      <FieldDescription>Indicates whether the spell requires concentration to maintain.</FieldDescription>
                    </FieldContent>
                  </Field>
                </FieldGroup>
              </FieldSet>

              {/* <Field orientation="horizontal">
                <Button type="submit">Submit</Button>
                <Button variant="outline" type="button">
                  Cancel
                </Button>
              </Field> */}
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}

function SpellLevelExample() {
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

  return (
    <form>
      <FieldGroup>
        <FieldSet>
          {/*  */}
          <FieldGroup className="">
            <div className="grid grid-cols-3 gap-4">
              <Field className="col-span-1">
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

              <Field className="col-span-2">
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
      </FieldGroup>
    </form>
  );
}

function NameFieldGroup() {
  return (
    <FieldGroup>
      <FieldSet>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="input-name">
              Name
              {/* <InfoPopover description="A spell costs a Magic action to cast, but some spells require a Bonus Action, a Reaction, or 1 minute or more." /> */}
            </FieldLabel>
            <Input
              id="input-name"
              placeholder="Fireball"
            />
            {/* <FieldDescription>The time required to cast the spell.</FieldDescription> */}
          </Field>
        </FieldGroup>
      </FieldSet>
    </FieldGroup>
  );
}
function CastingTimeExample() {
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
    <form>
      <FieldGroup>
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
      </FieldGroup>
    </form>
  );
}

function RangeExample() {
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
    <Example
      title="Range"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-md"
        size="default"
      >
        {/* <CardHeader className="border-b">
          <CardTitle>Range</CardTitle>
          <CardDescription>The range of the spell.</CardDescription>
        </CardHeader> */}
        <CardContent>
          <form>
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
            <RangeFieldGroup />
          </form>
        </CardContent>
      </Card>
    </Example>
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

function CastingTimeExample2() {
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
    <Example
      title="Casting Time + Header"
      className="items-center justify-center"
    >
      <Card
        className="w-full max-w-md"
        size="sm"
      >
        <CardHeader className="border-b">
          <CardTitle>Casting Time</CardTitle>
          <CardDescription>The time required to cast the spell.</CardDescription>
        </CardHeader>
        <CardContent>
          <form>
            <FieldGroup>
              <FieldSet>
                {/* <FieldLegend>Casting Time</FieldLegend>
                <FieldDescription>The time required to cast the spell.</FieldDescription> */}
                <FieldGroup>
                  <Field>
                    {/* <FieldLabel htmlFor="input-casting-time">Casting Time</FieldLabel> */}
                    <Autocomplete
                      id="input-casting-time"
                      items={times}
                    >
                      <AutocompleteInput placeholder="e.g. Bonus Action">
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

                    {/* <FieldDescription>You can enter any casting time.</FieldDescription> */}
                  </Field>

                  {/* <FieldSeparator /> */}
                  <Field orientation="horizontal">
                    <Checkbox id="is-ritual-spell-2" />
                    <FieldContent>
                      <FieldLabel
                        htmlFor="is-ritual-spell-2"
                        className="font-normal"
                      >
                        Ritual Spell
                      </FieldLabel>
                      <FieldDescription>Indicates whether the spell can be cast as a ritual.</FieldDescription>
                    </FieldContent>
                  </Field>
                </FieldGroup>
              </FieldSet>
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
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

const classes = ["Wizard", "Fighter", "Rogue", "Cleric", "Ranger", "Paladin", "Bard", "Druid", "Warlock", "Sorcerer", "Monk", "Barbarian"];

function AssignClasses() {
  const anchor = useComboboxAnchor();

  return (
    <Card
      className="w-full"
      size="default"
    >
      <CardHeader className="border-b">
        <CardTitle className="text-sm">Assign Classes</CardTitle>
        <CardDescription className="text-sm">Select classes to assign to this element.</CardDescription>
        {/* <CardAction>
            <Tooltip>
              <TooltipTrigger render={<Button variant="outline" size="icon-xs" />}>
                <PlusIcon />
              </TooltipTrigger>
              <TooltipContent>Add class</TooltipContent>
            </Tooltip>
          </CardAction> */}
      </CardHeader>
      <CardContent>
        <Combobox
          multiple
          autoHighlight
          items={classes}
          defaultValue={[classes[0]]}
        >
          <ComboboxChips ref={anchor}>
            <ComboboxValue>
              {(values) => (
                <React.Fragment>
                  {values.map((username: string) => (
                    <ComboboxChip key={username}>
                      {/* <Avatar className="size-4">
                          <AvatarImage src={`https://github.com/${username}.png`} alt={username} />
                          <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                        </Avatar> */}
                      {username}
                    </ComboboxChip>
                  ))}
                  <ComboboxChipsInput placeholder={values.length > 0 ? undefined : "Select a class..."} />
                </React.Fragment>
              )}
            </ComboboxValue>
          </ComboboxChips>
          <ComboboxContent anchor={anchor}>
            <ComboboxEmpty>No classes found.</ComboboxEmpty>
            <ComboboxList>
              {(username) => (
                <ComboboxItem
                  key={username}
                  value={username}
                >
                  {/* <Avatar className="size-5">
                      <AvatarImage src={`https://github.com/${username}.png`} alt={username} />
                      <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                    </Avatar> */}
                  {username}
                </ComboboxItem>
              )}
            </ComboboxList>
          </ComboboxContent>
        </Combobox>
      </CardContent>
    </Card>
  );
}

function RelatedAbilityScoreExample() {
  const abilities = [
    { label: "Strength", abbr: "STR", value: "id-1" },
    { label: "Dexterity", abbr: "DEX", value: "id-2" },
    { label: "Constitution", abbr: "CON", value: "id-3" },
    { label: "Intelligence", abbr: "INT", value: "id-4" },
    { label: "Wisdom", abbr: "WIS", value: "id-5" },
    { label: "Charisma", abbr: "CHA", value: "id-6" },
  ];

  return (
    <Example
      title="Skill Aspects"
      className="items-center justify-center"
    >
      <Card
        className="w-full"
        size="default"
      >
        {/* <CardHeader className="border-b">
          <CardTitle>Spell Details</CardTitle>
          <CardDescription>The classification of the spell within the magical schools.</CardDescription>
          <CardAction>
            <Tooltip>
              <TooltipTrigger render={<Button variant="outline" size="icon-xs" />}>
                <PlusIcon />
              </TooltipTrigger>
              <TooltipContent>Add School of Magic</TooltipContent>
            </Tooltip>
          </CardAction>
        </CardHeader> */}
        <CardContent>
          <form>
            <FieldGroup>
              <FieldSet>
                {/*  */}
                <FieldGroup className="">
                  <div className="grid grid-cols-3 gap-4">
                    <Field className="col-span-3">
                      <FieldLabel htmlFor="primary-ability">
                        Associated Ability Score
                        <InfoPopover description="The ability score associated with the skill according to the ruleset." />
                      </FieldLabel>
                      <Select
                        items={abilities}
                        defaultValue="id-1"
                      >
                        <SelectTrigger id="primary-ability">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectGroup>
                            {abilities.map((item) => (
                              <SelectItem
                                key={item.value}
                                value={item.value}
                              >
                                {item.label} <span className="text-muted-foreground text-xs mt-0.5">({item.abbr})</span>
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
            </FieldGroup>
          </form>
        </CardContent>
      </Card>
    </Example>
  );
}
