"use client";

import { useState } from "react";
import { GripIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

const initialNotifications = [
  {
    id: 1,
    image: "/portraits/portrait-1.jpg",
    user: "Chris Tompson",
    action: "requested review on",
    target: "PR #42: Feature implementation",
    timestamp: "15 minutes ago",
    unread: true,
  },
  {
    id: 2,
    image: "/portraits/portrait-2.jpg",
    user: "Emma Davis",
    action: "shared",
    target: "New component library",
    timestamp: "45 minutes ago",
    unread: true,
  },
  {
    id: 3,
    image: "/portraits/portrait-3.jpg",
    user: "James Wilson",
    action: "assigned you to",
    target: "API integration task",
    timestamp: "4 hours ago",
    unread: false,
  },
  {
    id: 4,
    image: "/portraits/portrait-4.jpg",
    user: "Alex Morgan",
    action: "replied to your comment in",
    target: "Authentication flow",
    timestamp: "12 hours ago",
    unread: false,
  },
  {
    id: 5,
    image: "/portraits/portrait-5.jpg",
    user: "Sarah Chen",
    action: "commented on",
    target: "Dashboard redesign",
    timestamp: "2 days ago",
    unread: false,
  },
  {
    id: 6,
    image: "/portraits/portrait-6.jpg",
    user: "Jarlaxle Baenre's",
    action: "added you to the campaign",
    target: "The Underdark Unveiled",
    timestamp: "2 weeks ago",
    unread: true,
  },
];

function Dot({ className }: { className?: string }) {
  return (
    <svg width="6" height="6" fill="currentColor" viewBox="0 0 6 6" xmlns="http://www.w3.org/2000/svg" className={className} aria-hidden="true">
      <circle cx="3" cy="3" r="3" />
    </svg>
  );
}

export default function AppMenuComponent() {
  const [notifications, setNotifications] = useState(initialNotifications);
  const unreadCount = notifications.filter((n) => n.unread).length;

  const handleMarkAllAsRead = () => {
    setNotifications(
      notifications.map((notification) => ({
        ...notification,
        unread: false,
      }))
    );
  };

  const handleNotificationClick = (id: number) => {
    setNotifications(notifications.map((notification) => (notification.id === id ? { ...notification, unread: false } : notification)));
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button size="icon-sm" variant="ghost" className="relative" aria-label="Open notifications">
          <GripIcon size={16} aria-hidden="true" />
          {/* {unreadCount > 0 && (
            <Badge variant="default" className="absolute -top-2 left-full min-w-5 -translate-x-1/2 px-1 bg-starlights-yellow-600 text-black">
              {unreadCount > 99 ? "99+" : unreadCount}
            </Badge>
          )} */}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-80 p-1" align="end">
        <div className="flex items-baseline justify-between gap-4 px-3 py-2">
          <div className="text-sm font-semibold">Notifications</div>
          {unreadCount > 0 && (
            <button className="text-xs font-medium hover:underline" onClick={handleMarkAllAsRead}>
              Mark all as read
            </button>
          )}
        </div>
        <div role="separator" aria-orientation="horizontal" className="-mx-1 my-1 h-px bg-border"></div>
        {notifications.map((notification) => (
          <div key={notification.id} className="rounded-md px-3 py-2 text-sm transition-colors hover:bg-accent">
            <div className="relative flex items-start gap-3 pe-3">
              <img className="size-9 rounded-md" src={notification.image} width={32} height={32} alt={notification.user} />
              <div className="flex-1 space-y-1">
                <button className="text-left text-foreground/80 after:absolute after:inset-0" onClick={() => handleNotificationClick(notification.id)}>
                  <span className="font-medium text-foreground hover:underline">{notification.user}</span> {notification.action}{" "}
                  <span className="font-medium text-foreground hover:underline">{notification.target}</span>.
                </button>
                <div className="text-xs text-muted-foreground">{notification.timestamp}</div>
              </div>
              {notification.unread && (
                <div className="absolute end-0 self-center">
                  <Dot />
                </div>
              )}
            </div>
          </div>
        ))}
      </PopoverContent>
    </Popover>
  );
}
