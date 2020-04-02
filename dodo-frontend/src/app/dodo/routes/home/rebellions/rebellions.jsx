import React, { Fragment } from 'react'

import { Header } from "./header"
import { List, ListContainer } from 'app/components/events'

export const Rebellions = ({ rebellions }) =>
	<ListContainer
	content={
		<Fragment>
			<Header/>
			<List events={rebellions}/>
		</Fragment>
	}
	/>




