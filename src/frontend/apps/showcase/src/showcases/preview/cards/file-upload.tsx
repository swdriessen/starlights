"use client"

import { UploadCloudIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "ui-framework/card"
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from "ui-framework/empty"

export function FileUpload() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>File Upload</CardTitle>
        <CardDescription>Drag and drop or browse</CardDescription>
      </CardHeader>
      <CardContent>
        <Empty className="border">
          <EmptyHeader>
            <EmptyMedia variant="icon">
              <UploadCloudIcon />
            </EmptyMedia>
            <EmptyTitle>Upload files</EmptyTitle>
            <EmptyDescription>PNG, JPG, PDF up to 10MB</EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Button>Browse Files</Button>
          </EmptyContent>
        </Empty>
      </CardContent>
    </Card>
  )
}
