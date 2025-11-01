import { useCharacterCards, useDeleteCharacter } from "@/lib/api/characters/queries";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { MoreVertical } from "lucide-react";
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

function CharacterCardCollectionSquares() {
  const { data: characterCards } = useCharacterCards();
  const deleteCharacter = useDeleteCharacter();

  return (
    <div>
      {characterCards && (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 2xl:grid-cols-7 gap-4">
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
                        Level {card.level} {card.build}
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
    </>
  );
}
