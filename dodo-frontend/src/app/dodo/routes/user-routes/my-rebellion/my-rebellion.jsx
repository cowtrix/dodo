import React, { useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container } from 'app/components/forms/index'
import { Resource } from './resource'


const MY_REBELLION = "My rebellion"

export const MyRebellion = ({ error, getMyRebellion, isFetching, myRebellion, resourceTypes }) => {
	useEffect(() => { getMyRebellion() }, []);

	let content;
	if(isFetching) {
		content = <>Loading...</>;
	}
	else if(error) {
		content = <>There was an error loading the data.</>;
	}
	else {
		content = (
			<>
				{/* {memberOf.map(resource =>
					<Resource {...resource} resourceTypes={resourceTypes}/>
				)} */}
				{'data = ' + JSON.stringify(myRebellion)}
			</>
		)
	}

	return (
		<Container title={MY_REBELLION} content={content} />
	)
}


MyRebellion.propTypes = {
	user: PropTypes.object
}
