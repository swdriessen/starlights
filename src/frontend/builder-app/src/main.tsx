import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
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
// import "./styles/typography.css";
import { DevelopmentPage } from "./pages/development/Index.tsx";
import { LibraryDevelopmentPage } from "./pages/development/library-page.tsx";
import BuilderPage, { CharacterDetailsPage } from "./pages/characters/builder/index.tsx";
import App, { AppWide } from "./App.tsx";
import BuilderLayout from "./pages/layouts/builder-layout.tsx";
import CharactersLayout from "./pages/layouts/builder-layout.tsx";
import { AppSidebar } from "@starlights/ui/components/app-sidebar.tsx";
import BuilderAppLayout from "./pages/layouts/builder-app-layout.tsx";
import BuilderAppLayout2 from "./pages/layouts/builder-app-layout-2.tsx";

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
    path: "/app",
    element: <BuilderAppLayout />,
    index: true,
  },
  {
    path: "/app2",
    element: <BuilderAppLayout2 />,
    index: true,
  },
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
    element: <AppWide />,
    children: [
      { index: true, element: <CharactersPage /> },
      { path: ":id", element: <CharactersDetailsPage /> },
      { path: "create", element: <CharactersCreatePage /> },
    ],
  },
  {
    path: "/characters/:id/builder",
    element: <CharactersLayout />,
    children: [{ index: true, element: <CharacterDetailsPage /> }],
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
