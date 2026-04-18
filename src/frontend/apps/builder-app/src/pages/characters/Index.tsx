import { useCharacterCards, useDeleteCharacter, type CharacterCard } from "@/lib/api/characters/queries";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardAction, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { AnvilIcon, FileSpreadsheetIcon, MenuIcon, PlusIcon, SwordsIcon, Trash2Icon } from "lucide-react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { ButtonGroup } from "@/components/ui/button-group";
import { Drawer, DrawerContent, DrawerDescription, DrawerHeader, DrawerTitle, DrawerTrigger } from "@/components/ui/drawer";
import { useIsMobile } from "@/hooks/use-mobile";
import { Empty, EmptyDescription, EmptyHeader, EmptyMedia, EmptyTitle } from "@/components/ui/empty";

import { CardWrapper } from "./components/card-wrapper";

function CharacterItem({
  title,
  description,
  url,
  image,
  size = "lg",
  tag = undefined,
  enabled = true,
}: {
  title: string;
  description: string;
  url: string;
  image?: string;
  size?: "sm" | "lg";
  tag?: string;
  enabled?: boolean;
}) {
  const isLarge = size === "lg";

  return (
    <>
      <div className="relative rounded-xl hover:shadow">
        <Link to={url} className="block relative aspect-square overflow-hidden rounded-xl group border-4 border-double ">
          <img
            src={image || "https://www.dndbeyond.com/attachments/12/424/flash-sale.jpg"}
            alt={title}
            className={`size-full aspect-square object-cover transition-transform duration-500 scale-100 group-hover:scale-110 ${
              enabled ? "" : "grayscale-100 group-hover:grayscale-0 "
            }`}
          />
          <div className="absolute inset-0 bg-linear-to-tr from-black/80 group-hover:from-black/50 to-transparent" />
          <div
            className={`prose prose-neutral dark:prose-invert absolute text-white ${
              isLarge ? "left-4 right-4 bottom-4 sm:left-8 sm:right-8 sm:bottom-8" : "left-2 right-2 bottom-2"
            }  `}
          >
            {isLarge ? (
              <>
                <h3 className="text-lg! sm:text-3xl! text-white mb-0">{title}</h3>
                <p className="text-sm sm:text-base">{description}</p>
              </>
            ) : (
              <>
                <h3 className="text-base sm:text-base! text-white mb-0 leading-5">{title}</h3>
                <p className="text-xs opacity-60 uppercase">{description}</p>
              </>
            )}
          </div>
          <div className={` absolute ${isLarge ? "left-4 right-4 top-4 sm:left-8 sm:right-8 sm:top-8" : "left-4 right-4 top-4"}  `}>
            {tag === undefined ? null : (
              <>
                <Badge variant={"outline"} className="backdrop-blur text-white border-white/50">
                  {tag}
                </Badge>
              </>
            )}
          </div>
        </Link>
        {/* <div>
          <Button variant={"ghost"} size="icon-sm" onClick={() => (enabled = !enabled)} className="absolute end-2 top-2 rounded-md ">
            <HeartIcon className={cn("size-4", enabled ? "fill-destructive" : "stroke-white")} />
            <span className="sr-only">Like</span>
          </Button>
        </div> */}
      </div>
    </>
  );
}

function CharacterDetailsDrawer({ card }: { card: CharacterCard }) {
  const deleteCharacter = useDeleteCharacter();

  return (
    <>
      <Drawer>
        <DrawerTrigger>
          <Button size="icon" variant={"ghost"} className="absolute end-1 top-1 rounded-lg ">
            {/* <MoreVerticalIcon /> */}
            <MenuIcon />
            <span className="sr-only">Like</span>
          </Button>
        </DrawerTrigger>
        <DrawerContent>
          <DrawerHeader className="">
            {/*  */}
            <div>
              <Card className="p-2 border-none">
                <div className="flex items-center ">
                  <img src={card.portraitUrl} alt={card.name} className="size-14 rounded-lg object-cover border-2 border-double" />
                  <div className="ms-3 flex-grow flex flex-col gap-1 justify-start items-start overflow-hidden text-left">
                    <DrawerTitle className="text-nowrap  w-full text-ellipsis overflow-hidden ">{card.name}</DrawerTitle>
                    <DrawerDescription className=" text-xs w-full text-ellipsis overflow-hidden">{`Level ${card.level} | ${card.build}`}</DrawerDescription>
                  </div>
                </div>
              </Card>
              <Separator className="my-4" />
              <div className="flex flex-col gap-4">
                <div className="flex items-center gap-4">
                  <Button variant={"outline"} className="flex-1 border-none">
                    <AnvilIcon />
                    <span className="text-xs text-center">Build</span>
                  </Button>
                  <Button variant={"outline"} className="flex-1 border-none">
                    <FileSpreadsheetIcon />
                    <span className="text-xs text-center">Sheet</span>
                  </Button>
                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button variant={"destructive"} size={"icon"} disabled={deleteCharacter.isPending} className="">
                        <Trash2Icon />
                        {/* Delete
                        <span className="sr-only">Delete</span> */}
                        {/* <span className="text-xs text-center">Delete</span> */}
                      </Button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Delete character?</AlertDialogTitle>
                        <AlertDialogDescription>This action cannot be undone. This will permanently delete the character "{card.name}".</AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                          className={buttonVariants({ variant: "destructive" })}
                          disabled={deleteCharacter.isPending}
                          onClick={() =>
                            deleteCharacter.mutate(card.characterId, {
                              onSuccess: () => toast.success("Character deleted"),
                              onError: (err) => toast.error("Failed to delete character", { description: err.message }),
                            })
                          }
                        >
                          Confirm delete
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>

                {/* <div className="flex flex-col">
                  <Button variant={"ghost"} size={"icon-lg"} className="flex flex-col" disabled>
                    <StarIcon className="fill-amber-300 stroke-amber-300" />
                    <span className="text-xs text-center">Favorite</span>
                  </Button>
                </div> */}

                {/* <div className="flex flex-col">
                  <Button variant={"ghost"} size={"icon-lg"} className="flex flex-col ">
                    <FileSpreadsheetIcon />
                    <span className="text-xs text-center">Sheet</span>
                  </Button>
                </div> */}
              </div>
            </div>
          </DrawerHeader>
        </DrawerContent>
      </Drawer>
    </>
  );
}

function CharactersCollection() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();
  const isMobile = useIsMobile();

  return (
    <div>
      {/* <CharactersSectionHeader title="My Character Collection" /> */}

      {characterCards === undefined && (
        <div>
          <Empty className="border border-dashed">
            <EmptyHeader>
              <EmptyMedia variant="icon">
                {/* <Spinner /> */}
                <SwordsIcon className="animate-pulse stroke-secondary" />
                {/* <SwordsIcon className="animate-ping " /> */}
                {/* <SparkleIcon className="animate-pulse" /> */}
              </EmptyMedia>
              <EmptyTitle>Gathering Adventurers</EmptyTitle>
              <EmptyDescription>We are currently gathering your adventurers...</EmptyDescription>
            </EmptyHeader>
          </Empty>
        </div>
      )}

      {characterCards && (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 2xl:grid-cols-7 gap-4">
          {characterCards.characters.map((card) => (
            <div key={card.characterId} className="group relative">
              <CharacterItem
                key={card.characterId}
                title={card.name}
                description={`Level ${card.level} ${card.build}`}
                url={`/characters/${card.characterId}`}
                image={card.portraitUrl}
                size="sm"
              />

              {isMobile && (
                <div className="absolute top-0 end-0">
                  <CharacterDetailsDrawer card={card} />
                </div>
              )}

              {isMobile ? null : (
                <AlertDialog>
                  <AlertDialogTrigger asChild>
                    <Button
                      variant={"destructive"}
                      disabled={deleteCharacter.isPending}
                      className="absolute end-3 top-3 rounded-lg size-6 hidden group-hover:flex opacity-100 hover:opacity-100"
                    >
                      <Trash2Icon className="size-3.5" />
                      <span className="sr-only">Delete</span>
                    </Button>
                  </AlertDialogTrigger>
                  <AlertDialogContent>
                    <AlertDialogHeader>
                      <AlertDialogTitle>Delete character?</AlertDialogTitle>
                      <AlertDialogDescription>This action cannot be undone. This will permanently delete the character "{card.name}".</AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogCancel>Cancel</AlertDialogCancel>
                      <AlertDialogAction
                        className={buttonVariants({ variant: "destructive" })}
                        disabled={deleteCharacter.isPending}
                        onClick={() =>
                          deleteCharacter.mutate(card.characterId, {
                            onSuccess: () => toast.success("Character deleted"),
                            onError: (err) => toast.error("Failed to delete character", { description: err.message }),
                          })
                        }
                      >
                        Confirm delete
                      </AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default function CharactersPage() {
  return (
    <>
      <div className="container mx-auto px-4 mt-12">
        <CardWrapper>
          <Card className="rounded-lg shadow-none">
            <CardHeader className="border-b">
              <CardTitle className="mb-3">Character Collection</CardTitle>
              <CardDescription>Organize your band of heroes — manage their appearance, progress, and epic deeds.</CardDescription>
              <CardAction>
                <ButtonGroup>
                  <Button variant="outline">
                    <Link to="/characters/create" className="flex items-center gap-2">
                      <PlusIcon size={16} />
                      New Character
                    </Link>
                  </Button>
                  {/* <Separator orientation="vertical" />
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="default" size="icon-sm" aria-label="More Options" disabled>
                        <MoreHorizontalIcon />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuGroup>
                        <DropdownMenuItem>
                          <PlusIcon />
                          New Group
                        </DropdownMenuItem>
                      </DropdownMenuGroup>
                    </DropdownMenuContent>
                  </DropdownMenu> */}
                </ButtonGroup>
              </CardAction>
            </CardHeader>
            <CardContent>
              <CharactersCollection />
            </CardContent>
          </Card>
        </CardWrapper>
      </div>
    </>
  );
}
