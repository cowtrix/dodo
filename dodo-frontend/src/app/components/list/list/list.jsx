import React from "react"
import PropTypes from "prop-types"
import { Summary } from "../../resources"
import { Button } from '../../button'


import styles from "./list.module.scss"

const SEE_MORE = "See All..."
const SEE_LESS = "See Less..."

export const List = ({ resources = [], resourceTypes, listExpanded, setListExpanded, isExpandableList, isMasterList = false }) =>
	<ul className={`${styles.eventList} ${isMasterList ? styles.masterList : ''}`}>
		{resources.map(event => <Summary {...event} key={event.slug} resourceTypes={resourceTypes}/>)}
		{isExpandableList ?
			<li>
				<Button
					onClick={() => setListExpanded(!listExpanded)}
					variant="link"
					className={styles.button}
				>
					{listExpanded ? SEE_LESS : SEE_MORE}
				</Button>
			</li> :
			null
		}
	</ul>


List.propTypes = {
	resources: PropTypes.array,
	resourceTypes: PropTypes.array,
	listExpanded: PropTypes.bool,
	isExpandableList: PropTypes.bool,
	setListExpanded: PropTypes.func,
}
