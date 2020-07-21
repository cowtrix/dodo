import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { Container } from 'app/components/forms/index'
import { Resource } from './resource'


const MY_REBELLION = "My rebellion"

export const MyRebellion = ({ memberOf = [], resourceTypes }) =>
	<Container
		title={MY_REBELLION}
		content={
			<Fragment>
				{memberOf.map(resource =>
				<Resource {...resource} resourceTypes={resourceTypes}/>
				)}
			</Fragment>
		}
	/>


MyRebellion.propTypes = {
	user: PropTypes.object
}