import React from "react"
import { SiteMap } from "app/components"

const zoom = 2

export const Header = ({ rebellions }) => (
	<SiteMap sites={rebellions} zoom={zoom} />
)
