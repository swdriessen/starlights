import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

export function CharacterDescription({ children }: { children: string }) {
  return <div className="flex flex-col gap-2 text-sm text-muted-foreground">
    <ReactMarkdown remarkPlugins={[remarkGfm]} skipHtml components={{
      a: ({ children, href }) => <a href={href} className="underline underline-offset-4">{children}</a>,
      ul: ({ children }) => <ul className="list-disc pl-5">{children}</ul>,
      ol: ({ children }) => <ol className="list-decimal pl-5">{children}</ol>,
      table: ({ children }) => <div className="overflow-x-auto"><table>{children}</table></div>,
    }}>{children}</ReactMarkdown>
  </div>;
}
