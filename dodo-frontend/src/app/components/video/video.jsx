import React from 'react'
import PropTypes from 'prop-types'

export const Video = ({ videoEmbedURL }) =>
	videoEmbedURL ? <iframe
		title="Resource Video"
		width="100%"
		height="315"
		src={videoEmbedURL}
		frameBorder="0"
		allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture"
		allowFullScreen
	/> :
		null

Video.propTypes = {
	videoEmbedURL: PropTypes.string,
}