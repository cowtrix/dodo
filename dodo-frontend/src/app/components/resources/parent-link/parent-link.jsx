import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import styles from './parent-link.module.scss'


const PART_OF = "Part of the"

const parentLink = (parent) =>
	'/' + parent.type + '/' + parent.guid

export const ParentLink = ({ parent }) =>
	parent ?
		<Link className={styles.parentLink} variant="link" to={parentLink(parent)}>
			<h3 className={styles.intro}>
				{PART_OF}
			</h3>
			<h2 className={styles.parentName}>
				{parent.name}
			</h2>
		</Link> :
		null

ParentLink.propTypes = {
	parent: PropTypes.object,
}
