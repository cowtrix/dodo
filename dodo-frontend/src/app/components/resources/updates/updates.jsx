import React from 'react'
import PropTypes from 'prop-types'

import styles from './updates.module.scss'
import { useTranslation } from 'react-i18next';
import { timeFormatted, dateFormatted } from '../header/dates/services';

const UpdateMeta = ({timestamp}) => (
	<div className={styles.updateMeta}>{timeFormatted(timestamp, {timeStyle: 'short'})} - {dateFormatted(timestamp, { day: "2-digit", month: "long", year: "numeric"})}</div>)

export const Updates = ({notifications: {notifications}}) => {
	const { t } = useTranslation("ui");

	return (
		<div className={styles.container}>
			<h3 className={styles.title}>{t('notifications_title')}</h3>
			{notifications.map(({canDelete,
				guid,
				link,
				message,
				permissionLevel,
				source,
				timestamp,
				type}) => (
				<div className={styles.update} key={guid}>
					<UpdateMeta timestamp={timestamp}/>
					{message}
				</div>
			))}
		</div>
	);
}


Updates.propTypes = {
	notifications: PropTypes.object.isRequired,
}
