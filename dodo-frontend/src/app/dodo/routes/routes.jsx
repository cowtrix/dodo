import React from "react";
import { Switch } from "react-router";
import { Route, BrowserRouter as Router } from "react-router-dom";
import { Home, route as home } from "./home";

import Rebellion from "app/dodo/routes/rebellion";

export const Routes = () => (
	<Router>
		<Switch>
			<Route
				path={"/rebellion/:rebellionId"}
				component={Rebellion}
				exact
			/>
			<Route path={home} component={Home} />
		</Switch>
	</Router>
);
