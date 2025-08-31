import { useParams } from "react-router-dom";

export default function CharactersDetailsPage() {
  const { id } = useParams<{ id: string }>();
  return (
    <>
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Character Details</h2>
        <p>Character ID: {id}</p>
      </div>
      <hr className="my-4" />
      <div className="space-y-4">
        <p>Details coming soon.</p>
      </div>
    </>
  );
}
