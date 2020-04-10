import React, { Fragment } from "react"
import { Route } from "react-router-dom"
import { Switch } from "react-router"

import { Home, route as home } from "./home"
import { Search, route as search } from "./search"
import { Rebellion } from "./rebellion"
import { Site } from "./site"

export const Routes = () => (
	<Fragment>
		<Switch>
			<Route
				path={"/rebellion/:rebellionId"}
				component={Rebellion}
				exact
			/>
			<Route path={"/site/:siteId"} component={Site} exact />
			<Route path={home} component={Home} exact />
			<Route path={search} component={Search} />
		</Switch>
	</Fragment>
)
