import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App, { BuilderLayout } from "./App.tsx";
import { Toaster } from "@/components/ui/sonner";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ThemeProvider } from "./components/theme-provider.tsx";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { LandingPage2 } from "./pages/landing/Index.tsx";
import AboutPage from "./pages/about/Index.tsx";
import CharactersPage from "./pages/characters/Index.tsx";
import CharactersCreatePage from "./pages/characters/create/Index.tsx";
import CharactersDetailsPage from "./pages/characters/details/Index.tsx";
import "./index.css";
import "./styles/typography.css";
import { DevelopmentPage } from "./pages/development/Index.tsx";
import { LibraryDevelopmentPage } from "./pages/development/library-page.tsx";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      staleTime: 5_000,
      refetchOnWindowFocus: false,
    },
  },
});

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { index: true, element: <LandingPage2 /> },
      { path: "about", element: <AboutPage /> },
      { path: "development", element: <DevelopmentPage /> },
      { path: "lib", element: <LibraryDevelopmentPage /> },
    ],
  },
  {
    path: "/characters",
    element: <BuilderLayout />,
    children: [
      { index: true, element: <CharactersPage /> },
      { path: ":id", element: <CharactersDetailsPage /> },
      { path: "create", element: <CharactersCreatePage /> },
    ],
  },
]);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <ThemeProvider defaultTheme="dark" storageKey="starlights-ui-theme">
        <RouterProvider router={router} />
        {/* <ReactQueryDevtools initialIsOpen={false} buttonPosition="bottom-left" /> */}
        <Toaster />
      </ThemeProvider>
    </QueryClientProvider>
  </StrictMode>
);
