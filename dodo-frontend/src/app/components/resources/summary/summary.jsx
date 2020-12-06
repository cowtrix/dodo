import React from "react"
import PropTypes from "prop-types"

import { Link } from "react-router-dom"
import { Title } from "./title"
import { Description } from "./description"
import styles from "./summary.module.scss"
import { Tile } from './tile/tile'
import { Icon } from 'app/components'


export const Summary = (
	{
		name,
		startDate,
		endDate,
		publicDescription,
		slug,
		metadata,
		resourceTypes,
		parent,
	}) =>
	<li className={styles.eventSummmary}>
		<Link
			to={`${"/" + metadata.type.toLowerCase() + "/" + slug}`}
			className={styles.link}
		>
			<div className={styles.summaryLeft}>
				<Tile type={metadata.type} resourceTypes={resourceTypes} />
				<Title title={name} parent={parent} startDate={startDate} endDate={endDate} type={metadata.type} />
				<Description description={publicDescription + '...'}/>
			</div>
			<div className={styles.summaryRight}>
				<Icon icon="chevron-right"/>
			</div>
		</Link>
	</li>

Summary.propTypes = {
	name: PropTypes.string,
	location: PropTypes.object,
	startDate: PropTypes.instanceOf(Date),
	endDate: PropTypes.instanceOf(Date),
	summary: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
	metadata: PropTypes.object,
	resourceTypes: PropTypes.array,
}

