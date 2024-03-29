import React, { useState } from 'react'
import PropTypes from 'prop-types'
import styles from '../list/list.module.scss'
import { List } from '../list'

export const ExpandableList = ({ resources = [], title, resourceTypes }) => {
	const [ listExpanded, setListExpanded ] = useState(false)

	return (
		resources.length ?
			<>
				{title ? <h3 className={styles.title}>{title}</h3> : null}
				<div className={styles.listBox}>
					<List
						resources={listExpanded ? resources : resources.map((r, i) => i < 3 && r).filter(x =>  x)}
						resourceTypes={resourceTypes}
						setListExpanded={setListExpanded}
						listExpanded={listExpanded}
						isExpandableList={setListExpanded && resources.length > 3}
					/>
				</div>
			</>
			: null
	)
}

ExpandableList.propTypes = {
	resources: PropTypes.array,
	resourceTypes: PropTypes.array,
	title: PropTypes.string
}
