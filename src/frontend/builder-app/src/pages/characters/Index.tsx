import { useCharacterCards, useDeleteCharacter } from "@/lib/api/characters/queries";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Trash2, Pencil, MoreHorizontal, Heart, LucideHeart, HeartIcon, Delete, Trash, CircleX, MoveVertical, MoreVertical } from "lucide-react";
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
import { cn } from "@/lib/utils";

function CharacterCollection() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div>
      {characterCards && (
        <div className="grid grid-cols-1 md:grid-cols-1 lg:grid-cols-2 gap-4">
          {characterCards.characters.map((card) => (
            <div key={card.characterId} className="overflow-hidden bg-black/50 rounded-lg p-2 hover:shadow-lg transition-shadow">
              <div className="flex flex-row gap-4">
                <div>
                  {card.portraitUrl && (
                    <div className="rounded-sm overflow-hidden max-w-48 w-24 max-h-24">
                      <img
                        src={card.portraitUrl}
                        alt={card.name}
                        className="h-full w-full object-cover transition-transform duration-200 hover:scale-200"
                        loading="lazy"
                      />
                    </div>
                  )}
                </div>
                <div className="flex flex-col justify-between gap-4 w-full pt-1">
                  <div className="space-y-1">
                    {/* <p className="text-xs text-muted-foreground font-light uppercase">Human Rogue (3)</p> */}
                    <h3 className="text-lg font-semibold">{card.name}</h3>
                  </div>

                  <div className="flex flex-row justify-between gap-4 ">
                    <Button size={"sm"} variant="outline" disabled={deleteCharacter.isPending} title="Delete character" asChild>
                      <Link to={`/characters/${card.characterId}`} title="Edit character">
                        <Pencil className="h-4 w-4" />
                        <span className="text-xs">Manage Character</span>
                      </Link>
                    </Button>
                    <span></span>

                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button size={"sm"} variant="destructive" disabled={deleteCharacter.isPending} title="Delete character" className="w-8">
                          <Trash2 className="h-4 w-4" />
                          {/* <span className="text-xs">Delete</span> */}
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>Delete character?</AlertDialogTitle>
                          <AlertDialogDescription>
                            This action cannot be undone. This will permanently delete the character "{card.name}".
                          </AlertDialogDescription>
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
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function CharacterCardCollection() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div>
      {characterCards && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6 gap-4">
          {characterCards.characters.map((card) => (
            <div key={card.characterId}>
              <Card key={card.characterId} className="pt-0">
                <CardContent className="px-0">
                  <img src={card.portraitUrl} alt="Banner" className="aspect-video h-50 rounded-t-xl object-cover w-full" />

                  {/* <div className="relative  overflow-hidden">
                    {card.portraitUrl && (
                      <img src={card.portraitUrl} alt={card.name} className="object-cover w-24 h-24 transition-transform duration-200" loading="lazy" />
                    )}
                  </div> */}
                </CardContent>
                <CardHeader>
                  <CardTitle> {card.name}</CardTitle>
                  <CardDescription>Mountain Dwarf &bull; Barbarian &bull; Outlander</CardDescription>
                </CardHeader>
                <CardFooter className="gap-2 flex flex-row xs:flex-col xs:items-stretch">
                  {/* <Button>Manage Character</Button> */}
                  {/* <Button>
                    <Link to={`/characters/${card.characterId}`} title="Manage character">
                      Manage Character
                    </Link>
                  </Button> */}
                  <Link to={`/characters/${card.characterId}`}>
                    <Button className="w-full">Manage</Button>
                  </Link>
                  {/* <Button variant={"destructive"}>Remove</Button> */}
                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button variant={"destructive"} disabled={deleteCharacter.isPending}>
                        Remove
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
                </CardFooter>
              </Card>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function CharacterCardCollection2() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div>
      {characterCards && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3 gap-4">
          {characterCards.characters.map((card) => (
            <div key={card.characterId}>
              <Card key={card.characterId} className=" py-0 sm:flex-row sm:gap-0">
                <CardContent className="grow-0 p-2 max-w-50">
                  <img src={card.portraitUrl} alt="Banner" className="size-full rounded-md h-30 w-30   object-cover" />
                </CardContent>
                <div className="sm:min-w-54 flex flex-col justify-between grow-1">
                  <CardHeader className="px-2 pt-4">
                    {/* <CardTitle>The Nameless One</CardTitle> */}
                    <CardTitle>{card.name}</CardTitle>
                    <CardDescription className="text-sm">
                      {/* <div className="flex items-center gap-2">
                        <Badge variant="outline">Human</Badge>
                        <Badge variant="outline">Fighter</Badge>
                      </div> */}
                      <span>Human Fighter</span> &bull; <span>Guild Artisan</span>
                    </CardDescription>
                    {/* <CardDescription className="text-sm">Background &bull; Outlander</CardDescription> */}
                  </CardHeader>
                  <CardFooter className="gap-2 pe-2 pb-2 flex justify-end ">
                    {/* <Link to={`/characters/${card.characterId}`}>
                      <Button className="w-full">Manage</Button>
                    </Link> */}
                    <span></span>

                    {/* <Button size="sm" variant={"outline"}>
                      Manage
                    </Button> */}

                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant={"destructive"} size="sm" disabled={deleteCharacter.isPending} className="hidden">
                          Remove
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>Delete character?</AlertDialogTitle>
                          <AlertDialogDescription>
                            This action cannot be undone. This will permanently delete the character "{card.name}".
                          </AlertDialogDescription>
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

                    <Button size="icon" variant="ghost">
                      <MoreHorizontal />
                    </Button>
                  </CardFooter>
                </div>
              </Card>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

function CharacterCardCollectionSquares() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div>
      {characterCards && (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
          {characterCards.characters.map((card) => (
            <div key={card.characterId}>
              <Card key={card.characterId} className="p-0 group ">
                <CardContent className="relative overflow-hidden p-1.5">
                  <Link to={`/characters/${card.characterId}`}>
                    <img
                      src={card.portraitUrl}
                      alt="Banner"
                      className="size-full aspect-square rounded-md object-cover transition-transform duration-300 group-hover:scale-100"
                    />
                    <div className="m-1.5 absolute top-0 left-0 right-0 bottom-0 rounded-md flex flex-col justify-end p-1.5 px-2 bg-gradient-to-tr from-black/80 group-hover:from-black/50 to-transparent group-hover:to-black/10 gap-0.5">
                      <span className="text-sm xl:text-base font-semibold uppercase text-white leading-none">{card.name}</span>
                      <p className="text-xxs xl:text-xs uppercase text-white/50">
                        Level {card.level}
                        {card.build}
                      </p>
                    </div>
                  </Link>

                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <Button
                        size="sm"
                        disabled={deleteCharacter.isPending}
                        className="bg-white/50 hover:bg-white/100 absolute end-3 top-3 rounded-sm hidden group-hover:flex w-8"
                      >
                        {/* <Trash2 className="stroke-black/80 stroke-1" /> */}
                        <MoreVertical className="stroke-black/80 stroke-1" />
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

                  {/* <Button
                    size="icon"
                    onClick={() => (card.isFavorite = !card.isFavorite)}
                    className="bg-primary/10 hover:bg-primary/20 absolute end-4 top-4 rounded-full "
                  >
                    <HeartIcon className={cn("size-4", card.isFavorite ? "fill-destructive stroke-destructive" : "stroke-white")} />
                    <span className="sr-only">Like</span>
                  </Button> */}
                </CardContent>
              </Card>
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
      <div className="space-y-2">
        <h2>Characters</h2>
        <p>Your characters are ready for you to manage.</p>
      </div>

      <hr className="my-4" />
      <CharacterCardCollectionSquares />

      {/* <CharacterCollection /> */}

      {/* <hr className="my-4" /> */}

      {/* <CharacterCardCollection /> */}

      {/* <hr className="my-4" />

      <CharacterCardCollection2 /> */}
    </>
  );
}
