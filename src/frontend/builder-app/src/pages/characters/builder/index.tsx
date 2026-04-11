import { useParams } from "react-router-dom";
import { Card, CardContent } from "@/components/ui/card";

export function BuilderHeader({ id }: { id: string }) {
  return (
    <>
      <div className="mt-12 px-4 container mx-auto ">
        <Card>
          <CardContent>Character ID: {id}</CardContent>
        </Card>
      </div>
    </>
  );
}

export function CharacterDetailsPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <>
      <div className="m-4 border p-4">
        <p>This is the selection picker page placeholder.</p>
        <p>ID: {id}</p>
      </div>
    </>
  );
}

export default function BuilderPage() {
  // get character id from url params
  const { id } = useParams();

  if (!id) {
    return <div>No character ID provided</div>;
  }

  return (
    <>
      <div className="container mx-auto px-4 my-0 mb-0"></div>

      <div className="bg-card border-l ps-4 overflow-hidden fixed w-120 2xl:w-140 right-0  top-(--navigation-height)  hidden md:block">
        <div className="h-full">{/* <DescriptionPanel id={id} /> */}</div>
      </div>
    </>
  );
}
