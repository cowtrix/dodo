import React, {Fragment} from 'react'
import {Header} from "./header";
import {RebellionMap} from "./rebellion-map";

export const Dodo = ({ allRebellionsGet }) => {
	allRebellionsGet()
	return(
		<Fragment>
			<Header/>
			<RebellionMap/>
		</Fragment>
	)
}

